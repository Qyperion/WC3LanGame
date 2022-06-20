using System.Net;
using System.Net.Sockets;
using System.Timers;
using WC3LanGame.Warcraft3;
using WC3LanGame.Warcraft3.Types;
using Timer = System.Timers.Timer;

namespace WC3LanGame.Network
{
    internal delegate void FoundServerHandler(GameInfo server);

    internal class Browser
    {
        private const ushort DefaultWarcraftPort = 6112;

        private Socket _browseSocket;
        private byte[] _browsePacket;

        private readonly IPEndPoint _serverEP;
        private readonly IPEndPoint _clientEP = new IPEndPoint(IPAddress.Broadcast, DefaultWarcraftPort);
        private readonly Timer _queryTimer = new(1000);

        private readonly byte[] _bytesBuffer = new byte[512];
        private readonly int _proxyPort;

        private bool _querying;
        private HostInfo _hostInfo;

        public event FoundServerHandler FoundServer;
        public event Action QuerySent;

        public Browser(IPAddress serverAddress, int proxyPort, HostInfo hostInfo)
        {
            _proxyPort = proxyPort;
            _hostInfo = hostInfo;
            _serverEP = new IPEndPoint(serverAddress, DefaultWarcraftPort);

            _queryTimer.Elapsed += QueryTimer_Elapsed;

            UpdateBrowsePacket();
        }

        public void SetConfiguration(IPAddress serverAddress, HostInfo hostInfo)
        {
            _serverEP.Address = serverAddress;
            _hostInfo = hostInfo;

            UpdateBrowsePacket();
        }

        public void Run()
        {
            _browseSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _browseSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
            _browseSocket.EnableBroadcast = true;

            _queryTimer.Start();
        }

        public void Stop()
        {
            _queryTimer.Stop();
            _browseSocket.Close();
            _browseSocket = null;
        }

        private void QueryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ProcessResponses();

            if (_querying) return;
            _querying = true;

            SendQuery();

            _querying = false;

            ProcessResponses();
        }

        private void SendQuery()
        {
            _browseSocket.SendTo(_browsePacket, _serverEP);
            QuerySent?.Invoke();
        }

        private void ProcessResponses()
        {
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

                if (bufferLength == 0) 
                    break;

                GameInfo gameInfo = WarcraftPacketProcessor.ParseGameInfoPacket(packet);

                if (gameInfo == null)
                    continue;

                OnFoundServer(gameInfo);
                ModifyGameName(packet);
                ModifyGamePort(packet, _proxyPort);

                _browseSocket.SendTo(packet, SocketFlags.None, _clientEP);
            }
        }

        public void SendGameCancelled(byte gameId)
        {
            byte[] packet = WarcraftPacketProcessor.GenerateGameCancelledPacket(gameId);
            _browseSocket.SendTo(packet, _clientEP);
        }

        // The client wont update the player count unless this is sent
        private void SendGameAnnounce(GameInfo gameInfo)
        {
            byte[] packet = WarcraftPacketProcessor.GenerateGameAnnouncePacket(gameInfo);
            _browseSocket.SendTo(packet, _clientEP);
        }

        // Replace "Local Game" with "Proxy Game"
        // This will not work properly for other languages
        private static void ModifyGameName(byte[] response)
        {
            response[0x14] = (byte)'P';
            response[0x15] = (byte)'r';
            response[0x16] = (byte)'o';
            response[0x17] = (byte)'x';
            response[0x18] = (byte)'y';
        }

        private static void ModifyGamePort(byte[] response, int port)
        {
            response[^2] = (byte)(port & 0xff);
            response[^1] = (byte)(port >> 8);
        }

        private void UpdateBrowsePacket()
        {
            _browsePacket = WarcraftPacketProcessor.GenerateQueryForLanGamesPacket(_hostInfo);
        }

        private void OnFoundServer(GameInfo gameInfo)
        {
            FoundServer?.Invoke(gameInfo);
            SendGameAnnounce(gameInfo);
        }
    }
}
