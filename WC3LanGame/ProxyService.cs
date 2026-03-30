using System.Net;
using System.Net.Sockets;

using WC3LanGame.Network;
using WC3LanGame.Warcraft3.Types;

namespace WC3LanGame
{
    internal class ProxyService : IDisposable
    {
        private Listener _listener;
        private Browser _browser;
        private readonly List<TcpProxy> _proxies = [];

        private CancellationTokenSource _cts;
        private IPEndPoint _serverEndPoint;
        private bool _foundGame;
        private DateTime _lastFoundServer;

        public GameInfo CurrentGame { get; private set; }
        public IPAddress ServerAddress { get; private set; }
        public bool IsRunning => _cts is { IsCancellationRequested: false };

        public int ConnectionCount
        {
            get { lock (_proxies) return _proxies.Count; }
        }

        public event Action<GameInfo> GameFound;
        public event Action GameLost;
        public event Action<string> Notification;
        public event Action ConnectionCountChanged;

        /// <summary>
        /// Starts the proxy service. Throws SocketException if listener fails to bind,
        /// or InvalidOperationException if hostname cannot be resolved.
        /// </summary>
        public void Start(HostInfo hostInfo)
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            ServerAddress = NetworkScanner.ResolveHost(hostInfo.Hostname);
            if (ServerAddress == null)
                throw new InvalidOperationException($"Unable to resolve host: {hostInfo.Hostname}");

            _serverEndPoint = new IPEndPoint(ServerAddress, 0);

            // Start listener first — if this throws SocketException, let it propagate
            _listener = new Listener(GotConnection, token);
            _listener.Run();

            // Start browser
            _browser = new Browser(ServerAddress, _listener.LocalEndPoint.Port, hostInfo, token);
            _browser.FoundServer += OnFoundServer;
            _browser.QuerySent += OnQuerySent;
            _browser.Run();
        }

        public void Dispose()
        {
            _cts?.Cancel();

            _listener?.Dispose();

            lock (_proxies)
            {
                foreach (TcpProxy proxy in _proxies)
                    proxy.Dispose();
                _proxies.Clear();
            }

            if (_foundGame && CurrentGame != null)
            {
                try { _browser?.SendGameCancelled((byte)CurrentGame.GameId); }
                catch (SocketException) { }
                catch (ObjectDisposedException) { }
            }

            _browser?.Dispose();

            _listener = null;
            _browser = null;
            _foundGame = false;
            CurrentGame = null;

            _cts?.Dispose();
            _cts = null;
        }

        private void OnFoundServer(GameInfo gameInfo)
        {
            CurrentGame = gameInfo;
            _serverEndPoint.Port = gameInfo.Port;

            if (!_foundGame)
                Notification?.Invoke("Found game: " + gameInfo.Name);

            _foundGame = true;
            _lastFoundServer = DateTime.Now;

            GameFound?.Invoke(gameInfo);
        }

        private void OnQuerySent()
        {
            // We don't receive the "server cancelled" messages
            // because they are only ever broadcast to the host's LAN.
            if (!_foundGame)
                return;

            TimeSpan interval = DateTime.Now - _lastFoundServer;
            if (interval.TotalSeconds > 3)
                OnLostGame();
        }

        private void OnLostGame()
        {
            if (!_foundGame)
                return;

            try { _browser?.SendGameCancelled((byte)CurrentGame.GameId); }
            catch (SocketException) { }
            catch (ObjectDisposedException) { }

            _foundGame = false;

            Notification?.Invoke("Lost game");
            GameLost?.Invoke();
        }

        private void GotConnection(Socket clientSocket)
        {
            Notification?.Invoke($"Got a connection from {clientSocket.RemoteEndPoint}");

            TcpProxy proxy = new TcpProxy(clientSocket, _serverEndPoint, _cts?.Token ?? CancellationToken.None);
            proxy.ProxyDisconnected += ProxyDisconnected;
            lock (_proxies) _proxies.Add(proxy);

            proxy.Run();

            ConnectionCountChanged?.Invoke();
        }

        private void ProxyDisconnected(TcpProxy proxy)
        {
            Notification?.Invoke("Client disconnected");

            lock (_proxies) _proxies.Remove(proxy);
            proxy.Dispose();

            ConnectionCountChanged?.Invoke();
        }
    }
}
