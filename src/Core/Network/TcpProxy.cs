using System.Net;
using System.Net.Sockets;

namespace WC3LanGame.Core.Network;

internal sealed class TcpProxy : IDisposable
{
    private readonly Socket _serverSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private readonly Socket _clientSocket;
    private readonly EndPoint _serverEndPoint;
    private readonly CancellationToken _cancellationToken;
    private readonly List<Socket> _selectList = new(2);

    private Thread _listenSocketsThread;
    private bool _disposed;

    public event EventHandler ProxyDisconnected;

    public TcpProxy(Socket clientSocket, EndPoint serverEndPoint, CancellationToken cancellationToken = default)
    {
        _clientSocket = clientSocket;
        _serverEndPoint = serverEndPoint;
        _cancellationToken = cancellationToken;
    }

    public void Run()
    {
        _serverSocket.Connect(_serverEndPoint);

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
        var buffer = new byte[2048];

        while (!_cancellationToken.IsCancellationRequested)
        {
            _selectList.Clear();
            _selectList.Add(_clientSocket);
            _selectList.Add(_serverSocket);
            Socket.Select(_selectList, null, null, 1000000);

            foreach (var socket in _selectList)
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

                var destinationSocket = socket == _serverSocket ? _clientSocket : _serverSocket;
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
        var sent = 0;
        while (sent < length)
        {
            var n = socket.Send(buffer, sent, length - sent, SocketFlags.None);
            if (n == 0)
                throw new SocketException((int)SocketError.ConnectionReset);
            sent += n;
        }
    }
}
