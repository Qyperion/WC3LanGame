using System.Net;
using System.Net.Sockets;
using System.Timers;
using WC3LanGame.Network;
using WC3LanGame.Warcraft3;
using WC3LanGame.Warcraft3.Types;
using Timer = System.Timers.Timer;

namespace WC3LanGame
{
    public partial class MainForm : Form
    {
        private Listener _listener; // This waits for proxy connections
        private Browser _browser; // This sends game info queries to the server and forwards the responses to the client
        private readonly List<TcpProxy> _proxies = new(); // A collection of game proxies.  Usually we would only need 1 proxy.
        
        private HostInfo _hostInfo;
        private IPAddress _serverAddress;
        private IPEndPoint _serverEP;

        private readonly Timer _updateWC3RunningStatusTimer = new(1000);
        
        private bool _foundGame;
        private DateTime _lastFoundServer;
        private GameInfo _gameInfo;


        public MainForm()
        {
            InitializeComponent();
            InitSettingsComponent();
        }

        #region Controls
        private void InitSettingsComponent()
        {
            foreach (WarcraftVersion version in (WarcraftVersion[]) Enum.GetValues(typeof(WarcraftVersion)))
            {
                wc3VersionComboBox.Items.Add(new WarcraftVersionWrapper(version));
            }
            
            string installedVersion = WarcraftExecutable.GetInstalledWC3Version();
            var item = wc3VersionComboBox.Items.Cast<WarcraftVersionWrapper>()
                .FirstOrDefault(x => x.ToString() == installedVersion);

            if (item != null)
                wc3VersionComboBox.SelectedItem = item;

            foreach (WarcraftType gameType in (WarcraftType[]) Enum.GetValues(typeof(WarcraftType)))
            {
                gameTypeComboBox.Items.Add(gameType);
            }

            gameTypeComboBox.SelectedIndex = gameTypeComboBox.Items.Count - 1;

            UpdateWC3RunningStatus(null, null);
            _updateWC3RunningStatusTimer.Elapsed += UpdateWC3RunningStatus;
            _updateWC3RunningStatusTimer.Start();
        }

        private void UpdateWC3RunningStatus(object sender, ElapsedEventArgs e)
        {
            bool wc3Running = WarcraftExecutable.IsWC3ProcessRunning();
            string wc3ProcessRunningStatus = wc3Running
                ? "WC3 is running"
                : "WC3 isn't running";

            if (wc3ProcessRunningStatusLabel.InvokeRequired)
                wc3ProcessRunningStatusLabel.Invoke(() => wc3ProcessRunningStatusLabel.Text = wc3ProcessRunningStatus);
            else
                wc3ProcessRunningStatusLabel.Text = wc3ProcessRunningStatus;

            if (runWC3Button.InvokeRequired)
                runWC3Button.Invoke(() => runWC3Button.Visible = !wc3Running);
            else
                runWC3Button.Visible = !wc3Running;

            if (stopWC3Button.InvokeRequired)
                stopWC3Button.Invoke(() => stopWC3Button.Visible = wc3Running);
            else
                stopWC3Button.Visible = wc3Running;
        }

        private void runProxyButton_Click(object sender, EventArgs e)
        {
            WarcraftVersionWrapper version = (WarcraftVersionWrapper) wc3VersionComboBox.SelectedItem;
            WarcraftType gameType = (WarcraftType) gameTypeComboBox.SelectedItem;
            _hostInfo = new HostInfo
            {
                Hostname = hostAddressComboBox.Text,
                Version = version.Version,
                GameType = gameType,
            };

            RunProxy();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopProxy();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Normal;
            Focus();
        }

        private void Notify(string text)
        {
            notifyIcon.ShowBalloonTip(1000, "WC3 Proxy", text, ToolTipIcon.Info);
        }

        private void stopProxyButton_Click(object sender, EventArgs e)
        {
            StopProxy();
        }

        private void runWC3Button_Click(object sender, EventArgs e)
        {
            string message = WarcraftExecutable.RunWC3((WarcraftType) gameTypeComboBox.SelectedItem);
            Notify(message);
        }

        private void stopWC3Button_Click(object sender, EventArgs e)
        {
            string result = WarcraftExecutable.StopWC3ProcessRunning();
            Notify(result);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            var ipList = await NetworkScanner.FindAllActiveIpInAllLocalNetworks(scanningNetworkProgressBar);
            hostAddressComboBox.Items.AddRange(ipList);
            scanningNetworkLabel.Visible = false;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            ShowInTaskbar = (WindowState != FormWindowState.Minimized);
        }

        #endregion


        #region Logic

        private void ResetGameInfo()
        {
            Notify("Lost game");

            gameNameValueLabel.Text = "-";
            gamePortValueLabel.Text = "-";
            gameTypeValueLabel.Text = "-";
            mapNameValueLabel.Text = "-";
            mapSizeValueLabel.Text = "-";
            playersCountValueLabel.Text = "-";
            clientCountValueLabel.Text = "-";
            proxyActiveLabel.Visible = false;

            _serverEP.Port = 0;
            _foundGame = false;
        }

        private void DisplayGameInfo()
        {
            if (InvokeRequired)
            {
                Invoke(DisplayGameInfo);
                return;
            }

            if (!_foundGame) 
                Notify("Found game: " + _gameInfo.Name);

            gamePortValueLabel.Text = _gameInfo.Port.ToString();
            gameNameValueLabel.Text = _gameInfo.Name;
            gameTypeValueLabel.Text = _gameInfo.GameType.ToString();
            mapNameValueLabel.Text = _gameInfo.MapName;
            mapSizeValueLabel.Text = $"{_gameInfo.MapSizeCategory} ({_gameInfo.MapHeight} x {_gameInfo.MapWidth})";
            playersCountValueLabel.Text = $"{_gameInfo.CurrentPlayersCount} / {_gameInfo.PlayerSlotsCount} / {_gameInfo.SlotsCount}";
            proxyActiveLabel.Visible = true;

            _serverEP.Port = _gameInfo.Port;
        }

        private void StartBrowser()
        {
            _browser = new Browser(_serverAddress, _listener.LocalEndPoint.Port, _hostInfo);
            _browser.QuerySent += Browser_QuerySent;
            _browser.FoundServer += Browser_FoundServer;
            _browser.Run();
        }

        private void Browser_FoundServer(GameInfo gameInfo)
        {
            _gameInfo = gameInfo;
            DisplayGameInfo();

            _foundGame = true;
            _lastFoundServer = DateTime.Now;
        }

        private void Browser_QuerySent()
        {
            // We don't receive the "server cancelled" messages
            // because they are only ever broadcast to the host's LAN.
            if (!_foundGame) 
                return;

            TimeSpan interval = DateTime.Now - _lastFoundServer;
            if (interval.TotalSeconds > 3)
                OnLostGame();
        }

        private void OnLostGame()
        {
            _browser?.SendGameCancelled((byte)_gameInfo.GameId);

            if (_foundGame)
                Invoke(ResetGameInfo);
        }

        private void StartTcpProxy()
        {
            _listener = new Listener(GotConnection);
            try
            {
                _listener.Run();
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Unable to start listener\n" + ex.Message);
            }
        }

        private void GotConnection(Socket clientSocket)
        {
            string message = $"Got a connection from {clientSocket.RemoteEndPoint}";
            Notify(message);

            TcpProxy proxy = new TcpProxy(clientSocket, _serverEP);
            proxy.ProxyDisconnected += ProxyDisconnected;
            lock (_proxies) _proxies.Add(proxy);

            proxy.Run();

            UpdateClientCount();
        }

        private void UpdateClientCount()
        {
            if (InvokeRequired)
            {
                Invoke(UpdateClientCount);
                return;
            }

            lock (_proxies)
            {
                clientCountValueLabel.Text = _proxies.Count.ToString();
            }
        }

        private void ProxyDisconnected(TcpProxy proxy)
        {
            Notify("Client disconnected");

            lock (_proxies)
                if (_proxies.Contains(proxy)) _proxies.Remove(proxy);

            UpdateClientCount();
        }

        private void StopProxy()
        {
            if (_listener != null)
            {
                _listener.Stop();
                lock (_proxies) 
                {
                    foreach (TcpProxy p in _proxies)
                        p.Stop();

                    _proxies.Clear();
                }
            }

            if (_foundGame) 
                _browser?.SendGameCancelled((byte)_gameInfo.GameId);

            _browser?.Stop();

            runProxyButton.Visible = true;
            stopProxyButton.Visible = false;
            gameInfoTableLayoutPanel.Visible = false;
            proxyActiveLabel.Visible = false;
        }

        private void RunProxy()
        {
            _serverAddress = NetworkScanner.ResolveHost(_hostInfo.Hostname);
            _serverEP = new IPEndPoint(_serverAddress, 0);

            string description = _serverAddress.ToString() == _hostInfo.Hostname
                ? _hostInfo.Hostname
                : $"{_hostInfo.Hostname} ({_serverAddress})";

            hostAddressValueLabel.Text = description;

            StartTcpProxy();
            StartBrowser();

            _browser?.SetConfiguration(_serverAddress, _hostInfo);

            runProxyButton.Visible = false;
            stopProxyButton.Visible = true;
            gameInfoTableLayoutPanel.Visible = true;
        }

        #endregion
    }
}