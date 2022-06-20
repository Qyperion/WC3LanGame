using System.Net;
using System.Net.Sockets;

namespace WC3LanGame.Network
{
    internal delegate void GotConnectionDelegate(Socket clientSocket);

    internal class Listener
    {
        public IPEndPoint LocalEndPoint => (IPEndPoint)_listenSocket.LocalEndPoint;

        private Socket _listenSocket;
        private readonly GotConnectionDelegate _connectionHandler;
        private readonly IPAddress _address;
        private readonly AsyncCallback _acceptCallback;

        private int _port;
        private bool _stop;

        private Listener(IPAddress address, int port, GotConnectionDelegate connectionHandler)
        {
            _address = address;
            _port = port;
            _connectionHandler = connectionHandler;
            _acceptCallback = EndAccept;
        }

        public Listener(GotConnectionDelegate connectionHandler)
            : this(IPAddress.Any, 0, connectionHandler)
        {
        }

        public void Run()
        {
            _stop = false;

            IPEndPoint endPoint = new IPEndPoint(_address, _port);

            _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _listenSocket.Bind(endPoint);

            _port = LocalEndPoint.Port;
            _listenSocket.Listen(20);

            BeginAccept();
        }

        private void BeginAccept()
        {
            if (_stop) 
                return;

            _listenSocket.BeginAccept(_acceptCallback, null);
        }

        private void EndAccept(IAsyncResult result)
        {
            if (_stop) 
                return;

            try
            {
                Socket client = _listenSocket.EndAccept(result);
                _connectionHandler(client);
            }
            catch (ObjectDisposedException) { }
            
            BeginAccept();
        }

        public void Stop()
        {
            _stop = true;
            _listenSocket.Close();
            _listenSocket = null;
        }
    }
}
