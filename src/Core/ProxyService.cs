using System.Net;
using System.Net.Sockets;

using WC3LanGame.Core.Network;
using WC3LanGame.Core.Warcraft3.Types;

namespace WC3LanGame.Core
{
    public sealed class ProxyService : IDisposable
    {
        private Listener _listener;
        private Browser _browser;
        private readonly List<TcpProxy> _proxies = [];

        private CancellationTokenSource _cts;
        private CancellationToken _token;
        private readonly Lock _serverEndPointLock = new();
        private IPEndPoint _serverEndPoint;
        private bool _notifiedGameFound;
        private bool _disposed;

        public GameInfo CurrentGame { get; private set; }
        public IPAddress ServerAddress { get; private set; }
        public HostInfo LastHostInfo { get; private set; }
        public bool IsRunning => _cts is { IsCancellationRequested: false };
        public int LatencyMs => _browser?.LatencyMs ?? -1;

        public int ConnectionCount
        {
            get { lock (_proxies) return _proxies.Count; }
        }

        public event EventHandler<GameInfo> GameFound;
        public event EventHandler GameLost;
        public event EventHandler<string> Notification;
        public event EventHandler ConnectionCountChanged;
        public event EventHandler Faulted;

        /// <summary>
        /// Starts the proxy service. Throws SocketException if listener fails to bind,
        /// or InvalidOperationException if hostname cannot be resolved.
        /// </summary>
        public void Start(HostInfo hostInfo)
        {
            LastHostInfo = hostInfo;
            _cts = new CancellationTokenSource();
            _token = _cts.Token;

            ServerAddress = NetworkScanner.ResolveHost(hostInfo.Hostname);
            if (ServerAddress == null)
                throw new InvalidOperationException($"Unable to resolve host: {hostInfo.Hostname}");

            _serverEndPoint = new IPEndPoint(ServerAddress, 0);

            // Start listener first — if this throws SocketException, let it propagate
            _listener = new Listener(GotConnection, _token);
            _listener.Faulted += OnComponentFaulted;
            _listener.Run();

            // Start browser
            _browser = new Browser(ServerAddress, _listener.LocalEndPoint.Port, hostInfo, _token);
            _browser.GameFound += OnGameFound;
            _browser.GameLost += OnGameLost;
            _browser.Faulted += OnComponentFaulted;
            _browser.Run();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

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

            if (_browser != null)
            {
                _browser.GameFound -= OnGameFound;
                _browser.GameLost -= OnGameLost;
                _browser.Faulted -= OnComponentFaulted;
            }

            _browser?.Dispose();

            _listener = null;
            _browser = null;
            CurrentGame = null;

            _cts?.Dispose();
            _cts = null;
        }

        private void OnGameFound(GameInfo gameInfo)
        {
            CurrentGame = gameInfo;
            lock (_serverEndPointLock)
            {
                _serverEndPoint.Port = gameInfo.Port;
            }

            if (!_notifiedGameFound)
            {
                Notification?.Invoke(this, "Found game: " + gameInfo.Name);
                _notifiedGameFound = true;
            }

            GameFound?.Invoke(this, gameInfo);
        }

        private void OnGameLost()
        {
            CurrentGame = null;
            _notifiedGameFound = false;
            Notification?.Invoke(this, "Lost game");
            GameLost?.Invoke(this, EventArgs.Empty);
        }

        private void OnComponentFaulted()
        {
            // Ignore if already shutting down
            if (_cts == null || _cts.IsCancellationRequested)
                return;

            Faulted?.Invoke(this, EventArgs.Empty);
        }

        private void GotConnection(Socket clientSocket)
        {
            Notification?.Invoke(this, $"Got a connection from {clientSocket.RemoteEndPoint}");

            EndPoint serverEPSnapshot;
            lock (_serverEndPointLock)
            {
                // Snapshot the current server endpoint for this connection
                serverEPSnapshot = new IPEndPoint(
                    ((IPEndPoint)_serverEndPoint).Address,
                    ((IPEndPoint)_serverEndPoint).Port);
            }

            TcpProxy proxy = new TcpProxy(clientSocket, serverEPSnapshot, _token);
            proxy.ProxyDisconnected += ProxyDisconnected;
            lock (_proxies) _proxies.Add(proxy);

            try
            {
                proxy.Run();
            }
            catch (SocketException)
            {
                // Server unreachable — clean up this proxy
                Notification?.Invoke(this, "Failed to connect to game server");
                lock (_proxies) _proxies.Remove(proxy);
                proxy.Dispose();
            }

            ConnectionCountChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ProxyDisconnected(TcpProxy proxy)
        {
            Notification?.Invoke(this, "Client disconnected");

            lock (_proxies) _proxies.Remove(proxy);
            proxy.Dispose();

            ConnectionCountChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
