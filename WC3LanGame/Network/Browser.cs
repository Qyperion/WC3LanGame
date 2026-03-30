using System.Net;
using System.Net.Sockets;
using System.Timers;

using WC3LanGame.Warcraft3;
using WC3LanGame.Warcraft3.Types;

using Timer = System.Timers.Timer;

namespace WC3LanGame.Network
{
    internal class Browser : IDisposable
    {
        private const ushort DefaultWarcraftPort = 6112;
        private const int GameNameOffset = 0x14;
        private static readonly byte[] ProxyNameBytes = "Proxy"u8.ToArray();

        private Socket _browseSocket;
        private byte[] _browsePacket;
        private readonly Lock _socketLock = new();

        private readonly IPEndPoint _serverEndPoint;
        private readonly IPEndPoint _clientEndPoint = new(IPAddress.Broadcast, DefaultWarcraftPort);
        private readonly Timer _queryTimer = new(1000);

        private readonly byte[] _bytesBuffer = new byte[512];
        private readonly int _proxyPort;
        private readonly CancellationToken _cancellationToken;

        private readonly Lock _queryLock = new();
        private bool _querying;

        public event Action<GameInfo> FoundServer;
        public event Action QuerySent;

        public Browser(IPAddress serverAddress, int proxyPort, HostInfo hostInfo, CancellationToken cancellationToken = default)
        {
            _proxyPort = proxyPort;
            _cancellationToken = cancellationToken;
            _serverEndPoint = new IPEndPoint(serverAddress, DefaultWarcraftPort);

            _queryTimer.Elapsed += QueryTimer_Elapsed;

            _browsePacket = WarcraftPacketProcessor.GenerateQueryForLanGamesPacket(hostInfo);
        }

        public void Run()
        {
            lock (_socketLock)
            {
                _browseSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _browseSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                _browseSocket.EnableBroadcast = true;
            }

            _queryTimer.Start();
        }

        public void SendGameCancelled(byte gameId)
        {
            byte[] packet = WarcraftPacketProcessor.GenerateGameCancelledPacket(gameId);
            lock (_socketLock)
            {
                _browseSocket?.SendTo(packet, _clientEndPoint);
            }
        }

        public void Dispose()
        {
            _queryTimer.Stop();
            _queryTimer.Dispose();
            lock (_socketLock)
            {
                try { _browseSocket?.Close(); } catch (ObjectDisposedException) { }
                _browseSocket = null;
            }
        }

        private void QueryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_cancellationToken.IsCancellationRequested)
                return;

            List<GameInfo> foundGames = CollectResponses();
            NotifyFoundGames(foundGames);

            lock (_queryLock)
            {
                if (_querying) return;
                _querying = true;
            }

            try
            {
                SendQuery();

                foundGames = CollectResponses();
                NotifyFoundGames(foundGames);
            }
            finally
            {
                lock (_queryLock)
                {
                    _querying = false;
                }
            }
        }

        private void SendQuery()
        {
            lock (_socketLock)
            {
                _browseSocket?.SendTo(_browsePacket, _serverEndPoint);
            }
            QuerySent?.Invoke();
        }

        /// <summary>
        /// Reads all pending responses from the socket under lock. Returns parsed games.
        /// Events are fired OUTSIDE the lock by the caller to prevent deadlocks.
        /// </summary>
        private List<GameInfo> CollectResponses()
        {
            List<GameInfo> games = [];

            lock (_socketLock)
            {
                if (_browseSocket == null)
                    return games;

                while (_browseSocket.Poll(0, SelectMode.SelectRead))
                {
                    int bufferLength;
                    byte[] packet;
                    try
                    {
                        bufferLength = _browseSocket.Receive(_bytesBuffer);
                        packet = _bytesBuffer[..bufferLength];
                    }
                    catch (SocketException)
                    {
                        // "An existing connection was forcibly closed by the remote host"
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }

                    if (bufferLength == 0)
                        break;

                    GameInfo gameInfo = WarcraftPacketProcessor.ParseGameInfoPacket(packet);

                    if (gameInfo == null)
                        continue;

                    games.Add(gameInfo);
                    ModifyGameName(packet);
                    ModifyGamePort(packet, _proxyPort);

                    // Forward modified packet to LAN and send announce
                    _browseSocket.SendTo(packet, SocketFlags.None, _clientEndPoint);

                    byte[] announcePacket = WarcraftPacketProcessor.GenerateGameAnnouncePacket(gameInfo);
                    _browseSocket.SendTo(announcePacket, _clientEndPoint);
                }
            }

            return games;
        }

        private void NotifyFoundGames(List<GameInfo> games)
        {
            foreach (GameInfo game in games)
                FoundServer?.Invoke(game);
        }

        // Replace "Local Game" with "Proxy Game"
        // This will not work properly for other languages
        private static void ModifyGameName(byte[] response)
        {
            if (response.Length < GameNameOffset + ProxyNameBytes.Length)
                return;

            ProxyNameBytes.CopyTo(response, GameNameOffset);
        }

        private static void ModifyGamePort(byte[] response, int port)
        {
            if (response.Length < 2)
                return;

            response[^2] = (byte)(port & 0xff);
            response[^1] = (byte)(port >> 8);
        }
    }
}
