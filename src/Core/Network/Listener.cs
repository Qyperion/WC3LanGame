using System.Net;
using System.Net.Sockets;

namespace WC3LanGame.Core.Network
{
    internal sealed class Listener : IDisposable
    {
        public IPEndPoint LocalEndPoint => (IPEndPoint)_listenSocket.LocalEndPoint;

        public event EventHandler Faulted;

        private Socket _listenSocket;
        private readonly Action<Socket> _connectionHandler;
        private readonly IPAddress _address;
        private readonly CancellationToken _cancellationToken;

        private Task _acceptTask;
        private int _port;
        private bool _disposed;

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

            _acceptTask = AcceptLoopAsync();
            _acceptTask.ContinueWith(
                static (_, state) => ((Listener)state).Faulted?.Invoke(state, EventArgs.Empty),
                this,
                TaskContinuationOptions.OnlyOnFaulted);
        }

        private async Task AcceptLoopAsync()
        {
            bool faulted = false;
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
                    faulted = true;
                    break;
                }
                catch (SocketException)
                {
                    // Socket error during accept — stop listening
                    faulted = true;
                    break;
                }
            }

            if (faulted)
                Faulted?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { _listenSocket?.Close(); } catch (ObjectDisposedException) { }
            _listenSocket = null;
            try { _acceptTask?.Wait(TimeSpan.FromSeconds(2)); } catch { }
        }
    }
}
