using System.Net;
using System.Net.Sockets;

using WC3LanGame.Core.Network;
using WC3LanGame.Core.Warcraft3.Types;

namespace WC3LanGame.Core
{
    public class ProxyService : IDisposable
    {
        private Listener _listener;
        private Browser _browser;
        private readonly List<TcpProxy> _proxies = [];

        private CancellationTokenSource _cts;
        private readonly Lock _serverEndPointLock = new();
        private IPEndPoint _serverEndPoint;
        private bool _foundGame;
        private DateTime _lastFoundServer;

        public GameInfo CurrentGame { get; private set; }
        public IPAddress ServerAddress { get; private set; }
        public HostInfo LastHostInfo { get; private set; }
        public bool IsRunning => _cts is { IsCancellationRequested: false };

        public int ConnectionCount
        {
            get { lock (_proxies) return _proxies.Count; }
        }

        public event Action<GameInfo> GameFound;
        public event Action GameLost;
        public event Action<string> Notification;
        public event Action ConnectionCountChanged;
        public event Action Faulted;

        /// <summary>
        /// Starts the proxy service. Throws SocketException if listener fails to bind,
        /// or InvalidOperationException if hostname cannot be resolved.
        /// </summary>
        public void Start(HostInfo hostInfo)
        {
            LastHostInfo = hostInfo;
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            ServerAddress = NetworkScanner.ResolveHost(hostInfo.Hostname);
            if (ServerAddress == null)
                throw new InvalidOperationException($"Unable to resolve host: {hostInfo.Hostname}");

            _serverEndPoint = new IPEndPoint(ServerAddress, 0);

            // Start listener first — if this throws SocketException, let it propagate
            _listener = new Listener(GotConnection, token);
            _listener.Faulted += OnComponentFaulted;
            _listener.Run();

            // Start browser
            _browser = new Browser(ServerAddress, _listener.LocalEndPoint.Port, hostInfo, token);
            _browser.FoundServer += OnFoundServer;
            _browser.QuerySent += OnQuerySent;
            _browser.Faulted += OnComponentFaulted;
            _browser.Run();
        }

        public void Dispose()
        {
            _cts?.Cancel();

            if (_listener != null)
                _listener.Faulted -= OnComponentFaulted;

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

            // Unsubscribe events before disposing browser
            if (_browser != null)
            {
                _browser.FoundServer -= OnFoundServer;
                _browser.QuerySent -= OnQuerySent;
                _browser.Faulted -= OnComponentFaulted;
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
            lock (_serverEndPointLock)
            {
                _serverEndPoint.Port = gameInfo.Port;
            }

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

        private void OnComponentFaulted()
        {
            // Ignore if already shutting down
            if (_cts == null || _cts.IsCancellationRequested)
                return;

            Faulted?.Invoke();
        }

        private void GotConnection(Socket clientSocket)
        {
            Notification?.Invoke($"Got a connection from {clientSocket.RemoteEndPoint}");

            EndPoint serverEPSnapshot;
            lock (_serverEndPointLock)
            {
                // Snapshot the current server endpoint for this connection
                serverEPSnapshot = new IPEndPoint(
                    ((IPEndPoint)_serverEndPoint).Address,
                    ((IPEndPoint)_serverEndPoint).Port);
            }

            TcpProxy proxy = new TcpProxy(clientSocket, serverEPSnapshot, _cts?.Token ?? CancellationToken.None);
            proxy.ProxyDisconnected += ProxyDisconnected;
            lock (_proxies) _proxies.Add(proxy);

            try
            {
                proxy.Run();
            }
            catch (SocketException)
            {
                // Server unreachable — clean up this proxy
                Notification?.Invoke("Failed to connect to game server");
                lock (_proxies) _proxies.Remove(proxy);
                proxy.Dispose();
            }

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
