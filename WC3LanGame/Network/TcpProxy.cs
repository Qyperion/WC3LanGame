using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace WC3LanGame.Network
{
    internal delegate void ProxyDisconnectedHandler(TcpProxy proxy);

    internal class TcpProxy
    {
        private readonly Socket _serverSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private readonly Socket _clientSocket;
        private readonly EndPoint _serverEP;

        private Thread _listenSocketsThread;
        private bool _isRunning;

        public event ProxyDisconnectedHandler ProxyDisconnected;

        public TcpProxy(Socket clientSocket, EndPoint serverEP)
        {
            _clientSocket = clientSocket;
            _serverEP = serverEP;
        }

        public void Run()
        {
            _serverSocket.Connect(_serverEP);

            _isRunning = true;
            _listenSocketsThread = new Thread(ListenSockets);
            _listenSocketsThread.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            _listenSocketsThread?.Join();
        }

        private void ListenSockets()
        {
            byte[] buffer = new byte[2048];
            List<Socket> sockets = new List<Socket>() {_clientSocket, _serverSocket};

            while (_isRunning)
            {
                IList readSockets = new List<Socket>(sockets);
                Socket.Select(readSockets, null, null, 1000000);

                foreach (Socket socket in readSockets)
                {
                    int bufferLength = 0;
                    try
                    {
                        bufferLength = socket.Receive(buffer);
                    }
                    catch { }

                    if (bufferLength == 0)
                    {
                        _isRunning = false;
                        ProxyDisconnected?.Invoke(this);
                        break;
                    }

                    Socket destinationSocket = socket == _serverSocket ? _clientSocket : _serverSocket;
                    destinationSocket.Send(buffer, bufferLength, SocketFlags.None);
                }
            }
        }
    }
}
