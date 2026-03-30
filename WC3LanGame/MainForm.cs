using System.Net.Sockets;
using System.Timers;

using WC3LanGame.Extensions;
using WC3LanGame.Network;
using WC3LanGame.Warcraft3;
using WC3LanGame.Warcraft3.Types;

using Timer = System.Timers.Timer;

namespace WC3LanGame
{
    public partial class MainForm : Form
    {
        private ProxyService _proxyService;
        private CancellationTokenSource _scanCts;

        private readonly Timer _updateWC3RunningStatusTimer = new(3000);


        public MainForm()
        {
            InitializeComponent();
            InitSettingsComponent();
        }

        #region Controls
        private void InitSettingsComponent()
        {
            wc3VersionComboBox.Format += (_, e) =>
            {
                if (e.ListItem is WarcraftVersion v)
                    e.Value = v.Version();
            };

            foreach (WarcraftVersion version in Enum.GetValues<WarcraftVersion>())
                wc3VersionComboBox.Items.Add(version);

            string installedVersion = WarcraftExecutable.GetInstalledWC3Version();
            foreach (WarcraftVersion version in Enum.GetValues<WarcraftVersion>())
            {
                if (version.Version() == installedVersion)
                {
                    wc3VersionComboBox.SelectedItem = version;
                    break;
                }
            }

            foreach (WarcraftType gameType in Enum.GetValues<WarcraftType>())
                gameTypeComboBox.Items.Add(gameType);

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

            if (!IsHandleCreated)
            {
                wc3ProcessRunningStatusLabel.Text = wc3ProcessRunningStatus;
                runWC3Button.Visible = !wc3Running;
                stopWC3Button.Visible = wc3Running;
                return;
            }

            BeginInvoke(() =>
            {
                wc3ProcessRunningStatusLabel.Text = wc3ProcessRunningStatus;
                runWC3Button.Visible = !wc3Running;
                stopWC3Button.Visible = wc3Running;
            });
        }

        private void runProxyButton_Click(object sender, EventArgs e)
        {
            if (wc3VersionComboBox.SelectedItem is not WarcraftVersion version)
                return;

            WarcraftType gameType = (WarcraftType)gameTypeComboBox.SelectedItem;
            HostInfo hostInfo = new HostInfo
            {
                Hostname = hostAddressComboBox.Text,
                Version = version,
                GameType = gameType,
            };

            RunProxy(hostInfo);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scanCts?.Cancel();
            StopProxy();
            _updateWC3RunningStatusTimer.Stop();
            _updateWC3RunningStatusTimer.Dispose();
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
            string message = WarcraftExecutable.RunWC3((WarcraftType)gameTypeComboBox.SelectedItem);
            Notify(message);
        }

        private void stopWC3Button_Click(object sender, EventArgs e)
        {
            string result = WarcraftExecutable.StopWC3ProcessRunning();
            Notify(result);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            _scanCts = new CancellationTokenSource();
            try
            {
                var ipList = await NetworkScanner.FindAllActiveIpInAllLocalNetworks(
                    scanningNetworkProgressBar, _scanCts.Token);
                hostAddressComboBox.Items.AddRange(ipList);
            }
            catch (OperationCanceledException)
            {
                // Form closing during scan — ignore
            }
            scanningNetworkLabel.Visible = false;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            ShowInTaskbar = (WindowState != FormWindowState.Minimized);
        }

        #endregion


        #region Logic

        private void RunProxy(HostInfo hostInfo)
        {
            _proxyService = new ProxyService();
            _proxyService.GameFound += ProxyService_GameFound;
            _proxyService.GameLost += ProxyService_GameLost;
            _proxyService.Notification += ProxyService_Notification;
            _proxyService.ConnectionCountChanged += ProxyService_ConnectionCountChanged;

            try
            {
                _proxyService.Start(hostInfo);
            }
            catch (SocketException ex)
            {
                _proxyService.Dispose();
                _proxyService = null;
                MessageBox.Show("Unable to start listener\n" + ex.Message);
                return;
            }
            catch (InvalidOperationException ex)
            {
                _proxyService.Dispose();
                _proxyService = null;
                MessageBox.Show(ex.Message);
                return;
            }

            string description = _proxyService.ServerAddress.ToString() == hostInfo.Hostname
                ? hostInfo.Hostname
                : $"{hostInfo.Hostname} ({_proxyService.ServerAddress})";

            hostAddressValueLabel.Text = description;

            runProxyButton.Visible = false;
            stopProxyButton.Visible = true;
            gameInfoTableLayoutPanel.Visible = true;
        }

        private void StopProxy()
        {
            _proxyService?.Dispose();
            _proxyService = null;

            runProxyButton.Visible = true;
            stopProxyButton.Visible = false;
            gameInfoTableLayoutPanel.Visible = false;
            proxyActiveLabel.Visible = false;
        }

        private void ProxyService_GameFound(GameInfo gameInfo)
        {
            BeginInvoke(() =>
            {
                gamePortValueLabel.Text = gameInfo.Port.ToString();
                gameNameValueLabel.Text = gameInfo.Name;
                gameTypeValueLabel.Text = gameInfo.GameType.ToString();
                mapNameValueLabel.Text = gameInfo.MapName;
                mapSizeValueLabel.Text = $"{gameInfo.MapSizeCategory} ({gameInfo.MapHeight} x {gameInfo.MapWidth})";
                playersCountValueLabel.Text = $"{gameInfo.CurrentPlayersCount} / {gameInfo.PlayerSlotsCount} / {gameInfo.SlotsCount}";
                proxyActiveLabel.Visible = true;
            });
        }

        private void ProxyService_GameLost()
        {
            BeginInvoke(() =>
            {
                gameNameValueLabel.Text = "-";
                gamePortValueLabel.Text = "-";
                gameTypeValueLabel.Text = "-";
                mapNameValueLabel.Text = "-";
                mapSizeValueLabel.Text = "-";
                playersCountValueLabel.Text = "-";
                clientCountValueLabel.Text = "-";
                proxyActiveLabel.Visible = false;
            });
        }

        private void ProxyService_Notification(string text)
        {
            BeginInvoke(() => Notify(text));
        }

        private void ProxyService_ConnectionCountChanged()
        {
            BeginInvoke(() =>
            {
                clientCountValueLabel.Text = (_proxyService?.ConnectionCount ?? 0).ToString();
            });
        }

        #endregion
    }
}
