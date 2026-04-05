using System.Net;
using System.Net.Sockets;

namespace WC3LanGame.Core.Network
{
    internal sealed class TcpProxy : IDisposable
    {
        private readonly Socket _serverSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private readonly Socket _clientSocket;
        private readonly EndPoint _serverEP;
        private readonly CancellationToken _cancellationToken;
        private readonly List<Socket> _selectList = new(2);

        private Thread _listenSocketsThread;
        private bool _disposed;

        public event EventHandler ProxyDisconnected;

        public TcpProxy(Socket clientSocket, EndPoint serverEP, CancellationToken cancellationToken = default)
        {
            _clientSocket = clientSocket;
            _serverEP = serverEP;
            _cancellationToken = cancellationToken;
        }

        public void Run()
        {
            _serverSocket.Connect(_serverEP);

            _listenSocketsThread = new Thread(ListenSockets);
            _listenSocketsThread.Start();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { _serverSocket.Close(); } catch (ObjectDisposedException) { }
            try { _clientSocket.Close(); } catch (ObjectDisposedException) { }
            _listenSocketsThread?.Join(TimeSpan.FromSeconds(2));
        }

        private void ListenSockets()
        {
            byte[] buffer = new byte[2048];

            while (!_cancellationToken.IsCancellationRequested)
            {
                _selectList.Clear();
                _selectList.Add(_clientSocket);
                _selectList.Add(_serverSocket);
                Socket.Select(_selectList, null, null, 1000000);

                foreach (Socket socket in _selectList)
                {
                    int bufferLength;
                    try
                    {
                        bufferLength = socket.Receive(buffer);
                    }
                    catch (SocketException)
                    {
                        if (!_disposed) ProxyDisconnected?.Invoke(this, EventArgs.Empty);
                        return;
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }

                    if (bufferLength == 0)
                    {
                        if (!_disposed) ProxyDisconnected?.Invoke(this, EventArgs.Empty);
                        return;
                    }

                    Socket destinationSocket = socket == _serverSocket ? _clientSocket : _serverSocket;
                    try
                    {
                        SendAll(destinationSocket, buffer, bufferLength);
                    }
                    catch (SocketException)
                    {
                        if (!_disposed) ProxyDisconnected?.Invoke(this, EventArgs.Empty);
                        return;
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }
                }
            }
        }

        private static void SendAll(Socket socket, byte[] buffer, int length)
        {
            int sent = 0;
            while (sent < length)
            {
                int n = socket.Send(buffer, sent, length - sent, SocketFlags.None);
                if (n == 0)
                    throw new SocketException((int)SocketError.ConnectionReset);
                sent += n;
            }
        }
    }
}
