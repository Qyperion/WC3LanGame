using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Timers;

using WC3LanGame.Core.Warcraft3;
using WC3LanGame.Core.Warcraft3.Types;

using Timer = System.Timers.Timer;

namespace WC3LanGame.Core.Network
{
    internal sealed class Browser : IDisposable
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

        private readonly int _proxyPort;
        private readonly CancellationToken _cancellationToken;

        private readonly Lock _queryLock = new();
        private bool _querying;

        private GameInfo _currentGame;
        private bool _foundGame;
        private DateTime _lastFoundServer;
        private long _querySentTimestamp;
        private bool _disposed;
        private int _consecutiveSendFailures;

        public int LatencyMs { get; private set; } = -1;

        public event EventHandler<GameInfo> GameFound;
        public event EventHandler GameLost;
        public event EventHandler Faulted;

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

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _queryTimer.Stop();
            _queryTimer.Dispose();

            if (_foundGame && _currentGame != null)
            {
                try { SendGameCancelled(_currentGame.GameId); }
                catch (SocketException) { }
                catch (ObjectDisposedException) { }
            }

            lock (_socketLock)
            {
                try { _browseSocket?.Close(); } catch (ObjectDisposedException) { }
                _browseSocket = null;
            }
        }

        private void SendGameCancelled(uint gameId)
        {
            byte[] packet = WarcraftPacketProcessor.GenerateGameCancelledPacket(gameId);
            lock (_socketLock)
            {
                _browseSocket?.SendTo(packet, _clientEndPoint);
            }
        }

        private void QueryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_disposed || _cancellationToken.IsCancellationRequested)
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
                CheckGameLost();

                foundGames = CollectResponses();
                if (foundGames.Count > 0 && _querySentTimestamp > 0)
                    LatencyMs = (int)Stopwatch.GetElapsedTime(_querySentTimestamp).TotalMilliseconds;
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
            try
            {
                lock (_socketLock)
                {
                    _browseSocket?.SendTo(_browsePacket, _serverEndPoint);
                }
                _consecutiveSendFailures = 0;
                _querySentTimestamp = Stopwatch.GetTimestamp();
            }
            catch (SocketException)
            {
                _consecutiveSendFailures++;
                if (_consecutiveSendFailures >= 5)
                {
                    _queryTimer.Stop();
                    Faulted?.Invoke(this, EventArgs.Empty);
                }
                return;
            }
            catch (ObjectDisposedException)
            {
                _queryTimer.Stop();
                Faulted?.Invoke(this, EventArgs.Empty);
                return;
            }
        }

        /// <summary>
        /// Reads all pending responses from the socket under lock. Returns parsed games.
        /// Events are fired OUTSIDE the lock by the caller to prevent deadlocks.
        /// </summary>
        private List<GameInfo> CollectResponses()
        {
            List<GameInfo> games = [];
            byte[] buffer = new byte[512];

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
                        bufferLength = _browseSocket.Receive(buffer);
                        packet = buffer[..bufferLength];
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
            {
                _currentGame = game;
                _foundGame = true;
                _lastFoundServer = DateTime.Now;
                GameFound?.Invoke(this, game);
            }
        }

        private void CheckGameLost()
        {
            if (!_foundGame)
                return;

            TimeSpan interval = DateTime.Now - _lastFoundServer;
            if (interval.TotalSeconds > 3)
            {
                try { SendGameCancelled(_currentGame.GameId); }
                catch (SocketException) { }
                catch (ObjectDisposedException) { }

                _foundGame = false;
                _currentGame = null;
                LatencyMs = -1;
                GameLost?.Invoke(this, EventArgs.Empty);
            }
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
