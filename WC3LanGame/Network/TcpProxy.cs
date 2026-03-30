using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace WC3LanGame.Network
{
    internal class TcpProxy : IDisposable
    {
        private readonly Socket _serverSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private readonly Socket _clientSocket;
        private readonly EndPoint _serverEP;
        private readonly CancellationToken _cancellationToken;

        private Thread _listenSocketsThread;

        public event Action<TcpProxy> ProxyDisconnected;

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
            try { _serverSocket.Close(); } catch (ObjectDisposedException) { }
            try { _clientSocket.Close(); } catch (ObjectDisposedException) { }
            _listenSocketsThread?.Join(TimeSpan.FromSeconds(2));
        }

        private void ListenSockets()
        {
            byte[] buffer = new byte[2048];
            List<Socket> sockets = [_clientSocket, _serverSocket];

            while (!_cancellationToken.IsCancellationRequested)
            {
                IList readSockets = new List<Socket>(sockets);
                Socket.Select(readSockets, null, null, 1000000);

                foreach (Socket socket in readSockets)
                {
                    int bufferLength;
                    try
                    {
                        bufferLength = socket.Receive(buffer);
                    }
                    catch (SocketException)
                    {
                        ProxyDisconnected?.Invoke(this);
                        return;
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }

                    if (bufferLength == 0)
                    {
                        ProxyDisconnected?.Invoke(this);
                        return;
                    }

                    Socket destinationSocket = socket == _serverSocket ? _clientSocket : _serverSocket;
                    try
                    {
                        destinationSocket.Send(buffer, bufferLength, SocketFlags.None);
                    }
                    catch (SocketException)
                    {
                        ProxyDisconnected?.Invoke(this);
                        return;
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }
                }
            }
        }
    }
}
