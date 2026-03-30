using System.Net;
using System.Net.Sockets;

namespace WC3LanGame.Network
{
    internal class Listener : IDisposable
    {
        public IPEndPoint LocalEndPoint => (IPEndPoint)_listenSocket.LocalEndPoint;

        private Socket _listenSocket;
        private readonly Action<Socket> _connectionHandler;
        private readonly IPAddress _address;
        private readonly CancellationToken _cancellationToken;

        private int _port;

        private Listener(IPAddress address, int port, Action<Socket> connectionHandler, CancellationToken cancellationToken)
        {
            _address = address;
            _port = port;
            _connectionHandler = connectionHandler;
            _cancellationToken = cancellationToken;
        }

        public Listener(Action<Socket> connectionHandler, CancellationToken cancellationToken = default)
            : this(IPAddress.Any, 0, connectionHandler, cancellationToken)
        {
        }

        public void Run()
        {
            IPEndPoint endPoint = new IPEndPoint(_address, _port);

            _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _listenSocket.Bind(endPoint);

            _port = LocalEndPoint.Port;
            _listenSocket.Listen(20);

            _ = AcceptLoopAsync();
        }

        private async Task AcceptLoopAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Socket client = await _listenSocket.AcceptAsync(_cancellationToken);
                    _connectionHandler(client);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }
        }

        public void Dispose()
        {
            try { _listenSocket?.Close(); } catch (ObjectDisposedException) { }
            _listenSocket = null;
        }
    }
}
