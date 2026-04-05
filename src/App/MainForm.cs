using System.Net;
using System.Net.Sockets;
using System.Timers;

using WC3LanGame.Core;
using WC3LanGame.Core.Extensions;
using WC3LanGame.Core.Network;
using WC3LanGame.Core.Warcraft3;
using WC3LanGame.Core.Warcraft3.Types;

using Timer = System.Timers.Timer;

namespace WC3LanGame.App
{
    public partial class MainForm : Form
    {
        private ProxyService _proxyService;
        private CancellationTokenSource _scanCts;
        private readonly AppSettings _settings;

        private readonly Timer _updateWC3RunningStatusTimer = new(3000);

        // Reconnect state
        private readonly ReconnectManager _reconnectManager = new();
        private HostInfo _lastHostInfo;


        public MainForm()
        {
            InitializeComponent();
            _settings = AppSettings.Load();
            InitSettingsComponent();
            DarkMode.Apply(this);

            _reconnectManager.ReconnectScheduled += ReconnectManager_ReconnectScheduled;
            _reconnectManager.ReconnectRequested += ReconnectManager_ReconnectRequested;
        }

        #region Controls
        private void InitSettingsComponent()
        {
            wc3VersionComboBox.Format += (_, e) =>
            {
                if (e.ListItem is WarcraftVersion v)
                    e.Value = $" {v.Version()}";
            };

            gameTypeComboBox.Format += (_, e) =>
            {
                if (e.ListItem is WarcraftType t)
                    e.Value = t.Description();
            };

            foreach (WarcraftVersion version in Enum.GetValues<WarcraftVersion>())
                wc3VersionComboBox.Items.Add(version);

            // Restore saved version, fall back to auto-detected from registry
            if (_settings.Version is WarcraftVersion savedVersion)
            {
                wc3VersionComboBox.SelectedItem = savedVersion;
            }
            else
            {
                TrySelectInstalledVersion(_settings.WarcraftExecutablePath);
            }

            // If no version selected yet, default to first item
            if (wc3VersionComboBox.SelectedIndex < 0 && wc3VersionComboBox.Items.Count > 0)
                wc3VersionComboBox.SelectedIndex = 0;

            foreach (WarcraftType gameType in Enum.GetValues<WarcraftType>())
                gameTypeComboBox.Items.Add(gameType);

            // Restore saved game type, fall back to last item (TheFrozenThrone)
            if (_settings.GameType is WarcraftType savedGameType)
                gameTypeComboBox.SelectedItem = savedGameType;
            else
                gameTypeComboBox.SelectedIndex = gameTypeComboBox.Items.Count - 1;

            // Restore saved host address
            if (!string.IsNullOrWhiteSpace(_settings.HostAddress))
                hostAddressComboBox.Text = _settings.HostAddress;

            autoReconnectCheckBox.Checked = _settings.AutoReconnect;

            hostAddressComboBox.TextChanged += (_, _) => UpdateConnectButtonState();
            UpdateConnectButtonState();

            UpdateWC3RunningStatus(null, null);
            _updateWC3RunningStatusTimer.Elapsed += UpdateWC3RunningStatus;
            _updateWC3RunningStatusTimer.Start();
        }

        private void UpdateWC3RunningStatus(object sender, ElapsedEventArgs e)
        {
            bool wc3Running = WarcraftExecutable.IsWC3ProcessRunning(_settings.WarcraftExecutablePath);
            string wc3ProcessRunningStatus = wc3Running
                ? "\u25CF WC3 is running"
                : "\u25CF WC3 isn't running";
            Color statusColor = wc3Running
                ? Color.LimeGreen
                : Color.Gray;

            if (!IsHandleCreated)
            {
                wc3ProcessRunningStatusLabel.Text = wc3ProcessRunningStatus;
                wc3ProcessRunningStatusLabel.ForeColor = statusColor;
                runWC3Button.Visible = !wc3Running;
                stopWC3Button.Visible = wc3Running;
                return;
            }

            BeginInvoke(() =>
            {
                wc3ProcessRunningStatusLabel.Text = wc3ProcessRunningStatus;
                wc3ProcessRunningStatusLabel.ForeColor = statusColor;
                runWC3Button.Visible = !wc3Running;
                stopWC3Button.Visible = wc3Running;
            });
        }

        private void runProxyButton_Click(object sender, EventArgs e)
        {
            if (wc3VersionComboBox.SelectedItem is not WarcraftVersion version)
                return;

            WarcraftType gameType = (WarcraftType)gameTypeComboBox.SelectedItem;
            HostInfo hostInfo = new HostInfo(hostAddressComboBox.Text, version, gameType);

            runProxyButton.Text = "Connecting...";
            runProxyButton.Enabled = false;
            runProxyButton.Update();

            RunProxy(hostInfo);

            // If RunProxy failed (button still visible), restore it
            if (runProxyButton.Visible)
            {
                runProxyButton.Text = "Connect";
                UpdateConnectButtonState();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _settings.HostAddress = hostAddressComboBox.Text;
            _settings.Version = wc3VersionComboBox.SelectedItem as WarcraftVersion?;
            _settings.GameType = gameTypeComboBox.SelectedItem as WarcraftType?;
            _settings.AutoReconnect = autoReconnectCheckBox.Checked;
            _settings.LogExpanded = logPanel.Visible;
            _settings.Save();

            _scanCts?.Cancel();
            StopProxy();
            _reconnectManager.Dispose();
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

        private enum LogLevel { Info, Success, Warning, Error }

        private void Log(string message, LogLevel level = LogLevel.Info)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string line = $"[{timestamp}] {message}{Environment.NewLine}";

            if (InvokeRequired)
            {
                BeginInvoke(() => AppendLog(line, message, level));
                return;
            }

            AppendLog(line, message, level);
        }

        private void AppendLog(string line, string statusText, LogLevel level)
        {
            // Keep log from growing too large (max ~500 lines)
            if (logRichTextBox.Lines.Length > 500)
            {
                logRichTextBox.SelectAll();
                logRichTextBox.SelectedText = "";
            }

            Color color = level switch
            {
                LogLevel.Success => Color.FromArgb(0, 160, 0),
                LogLevel.Warning => Color.FromArgb(200, 160, 0),
                LogLevel.Error => Color.FromArgb(220, 50, 50),
                _ => logRichTextBox.ForeColor
            };

            logRichTextBox.SelectionStart = logRichTextBox.TextLength;
            logRichTextBox.SelectionLength = 0;
            logRichTextBox.SelectionColor = color;
            logRichTextBox.AppendText(line);
            logRichTextBox.ScrollToCaret();

            // Update status bar with latest message
            lastLogStatusLabel.Text = statusText;
        }

        private void stopProxyButton_Click(object sender, EventArgs e)
        {
            StopProxy();
        }

        private void runWC3Button_Click(object sender, EventArgs e)
        {
            WarcraftType? warcraftType = gameTypeComboBox.SelectedItem as WarcraftType?;
            string executablePath = WarcraftExecutable.ResolveExecutablePath(
                _settings.WarcraftExecutablePath,
                warcraftType);

            if (string.IsNullOrWhiteSpace(executablePath))
            {
                executablePath = PromptForWarcraftExecutable();
                if (string.IsNullOrWhiteSpace(executablePath))
                {
                    Log("Warcraft III executable was not selected.", LogLevel.Warning);
                    return;
                }

                _settings.WarcraftExecutablePath = executablePath;
                _settings.Save();
                TrySelectInstalledVersion(executablePath);
            }

            string message = WarcraftExecutable.RunWC3(executablePath);
            Log(message);
            Notify(message);
        }

        private void stopWC3Button_Click(object sender, EventArgs e)
        {
            string result = WarcraftExecutable.StopWC3ProcessRunning(_settings.WarcraftExecutablePath);
            Log(result);
            Notify(result);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            if (_settings.LogExpanded)
                SetLogExpanded(true);

            await ScanNetworkAsync();
        }

        private async Task ScanNetworkAsync()
        {
            if (_scanCts != null)
                await _scanCts.CancelAsync();
            _scanCts = new CancellationTokenSource();

            scanningNetworkLabel.Visible = true;
            scanningNetworkProgressBar.Visible = true;
            scanningNetworkProgressBar.Minimum = 0;
            scanningNetworkProgressBar.Maximum = 1000;
            scanningNetworkProgressBar.Value = 0;
            rescanButton.Enabled = false;

            var progress = new Progress<double>(fraction =>
            {
                scanningNetworkProgressBar.Value = (int)(fraction * 1000);
            });

            try
            {
                var ipList = await NetworkScanner.FindAllActiveIpInAllLocalNetworks(
                    progress, _scanCts.Token);

                // Preserve current text, replace dropdown items
                string currentText = hostAddressComboBox.Text;
                hostAddressComboBox.Items.Clear();
                hostAddressComboBox.Items.AddRange(ipList);
                hostAddressComboBox.Text = currentText;

                Log($"Network scan complete, found {ipList.Length} active hosts");
            }
            catch (OperationCanceledException)
            {
                // Scan cancelled — ignore
            }

            scanningNetworkProgressBar.Visible = false;
            scanningNetworkLabel.Visible = false;
            rescanButton.Enabled = true;
        }

        private async void rescanButton_Click(object sender, EventArgs e)
        {
            await ScanNetworkAsync();
        }

        private void showLogToolStripLabel_Click(object sender, EventArgs e)
        {
            SetLogExpanded(!logPanel.Visible);
        }

        private void SetLogExpanded(bool expanded)
        {
            logPanel.Visible = expanded;
            int contentBottom = expanded ? logPanel.Bottom : settingsPanel.Bottom;
            int padding = logPanel.Left;
            ClientSize = new Size(ClientSize.Width, contentBottom + padding + statusStrip.Height);
            showLogToolStripLabel.Text = expanded ? "Hide Log \u25B2" : "Show Log \u25BC";
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            ShowInTaskbar = (WindowState != FormWindowState.Minimized);
        }

        private void UpdateConnectButtonState()
        {
            string text = hostAddressComboBox.Text.Trim();
            runProxyButton.Enabled = !string.IsNullOrWhiteSpace(text) &&
                (IPAddress.TryParse(text, out _) ||
                 Uri.CheckHostName(text) != UriHostNameType.Unknown);
        }

        private void TrySelectInstalledVersion(string executablePath)
        {
            string installedVersion = WarcraftExecutable.GetInstalledWC3Version(executablePath);
            foreach (WarcraftVersion version in Enum.GetValues<WarcraftVersion>())
            {
                if (version.Version() != installedVersion)
                    continue;

                wc3VersionComboBox.SelectedItem = version;
                break;
            }
        }

        private string PromptForWarcraftExecutable()
        {
            using OpenFileDialog dialog = new()
            {
                Title = "Select Warcraft III executable",
                Filter = "Warcraft III executables|Warcraft III.exe;war3.exe|Executable files (*.exe)|*.exe|All files (*.*)|*.*",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                RestoreDirectory = true,
                FileName = "Warcraft III.exe"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return null;

            return dialog.FileName;
        }

        #endregion


        #region Logic

        private void UpdateConnectionStatus(string text, Color color)
        {
            connectionStatusLabel.Text = text;
            connectionStatusLabel.ForeColor = color;
        }

        /// <returns>true if proxy started successfully</returns>
        private bool RunProxy(HostInfo hostInfo)
        {
            _proxyService = new ProxyService();

            try
            {
                _proxyService.Start(hostInfo);
            }
            catch (SocketException ex)
            {
                _proxyService.Dispose();
                _proxyService = null;
                Log($"Unable to start listener: {ex.Message}", LogLevel.Error);
                if (!_reconnectManager.IsReconnecting)
                    MessageBox.Show("Unable to start listener\n" + ex.Message);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                _proxyService.Dispose();
                _proxyService = null;
                Log($"Error: {ex.Message}", LogLevel.Error);
                if (!_reconnectManager.IsReconnecting)
                    MessageBox.Show(ex.Message);
                return false;
            }

            // Subscribe events only after successful start
            _proxyService.GameFound += ProxyService_GameFound;
            _proxyService.GameLost += ProxyService_GameLost;
            _proxyService.Notification += ProxyService_Notification;
            _proxyService.ConnectionCountChanged += ProxyService_ConnectionCountChanged;
            _proxyService.Faulted += ProxyService_Faulted;

            _lastHostInfo = hostInfo;

            string description = _proxyService.ServerAddress.ToString() == hostInfo.Hostname
                ? hostInfo.Hostname
                : $"{hostInfo.Hostname} ({_proxyService.ServerAddress})";

            hostAddressValueLabel.Text = description;
            Log($"Proxy started, connecting to {description}", LogLevel.Success);

            runProxyButton.Visible = false;
            stopProxyButton.Visible = true;
            gameInfoTableLayoutPanel.Visible = true;
            hostAddressComboBox.Enabled = false;
            wc3VersionComboBox.Enabled = false;
            gameTypeComboBox.Enabled = false;
            rescanButton.Enabled = false;
            UpdateConnectionStatus("\u25CF Searching for game...", Color.FromArgb(255, 180, 0));
            return true;
        }

        private void StopProxy(bool userInitiated = true)
        {
            if (userInitiated)
                _reconnectManager.Stop();

            if (_proxyService != null)
            {
                _proxyService.GameFound -= ProxyService_GameFound;
                _proxyService.GameLost -= ProxyService_GameLost;
                _proxyService.Notification -= ProxyService_Notification;
                _proxyService.ConnectionCountChanged -= ProxyService_ConnectionCountChanged;
                _proxyService.Faulted -= ProxyService_Faulted;
                _proxyService.Dispose();
                _proxyService = null;
            }

            if (userInitiated)
            {
                Log("Proxy stopped");
                runProxyButton.Text = "Connect";
                runProxyButton.Visible = true;
                stopProxyButton.Visible = false;
                gameInfoTableLayoutPanel.Visible = false;
                UpdateConnectionStatus("Not connected", Color.Gray);
                hostAddressComboBox.Enabled = true;
                wc3VersionComboBox.Enabled = true;
                gameTypeComboBox.Enabled = true;
                rescanButton.Enabled = true;
                UpdateConnectButtonState();
            }
        }

        private void ProxyService_Faulted(object sender, EventArgs e)
        {
            BeginInvoke(() =>
            {
                if (_reconnectManager.IsReconnecting) return;

                if (!autoReconnectCheckBox.Checked)
                {
                    Log("Proxy connection lost", LogLevel.Error);
                    StopProxy();
                    return;
                }

                _reconnectManager.Start();
            });
        }

        private void ReconnectManager_ReconnectScheduled(int attempt, int delaySeconds)
        {
            BeginInvoke(() =>
            {
                Log($"Connection lost. Reconnecting in {delaySeconds}s (attempt {attempt})...", LogLevel.Warning);
                UpdateConnectionStatus($"\u25CF Reconnecting ({attempt})...", Color.FromArgb(255, 180, 0));
            });
        }

        private void ReconnectManager_ReconnectRequested()
        {
            BeginInvoke(AttemptReconnect);
        }

        private void AttemptReconnect()
        {
            if (!_reconnectManager.IsReconnecting) return;

            StopProxy(userInitiated: false);

            if (_lastHostInfo == null || !autoReconnectCheckBox.Checked)
            {
                _reconnectManager.Stop();
                Log("Reconnection cancelled", LogLevel.Warning);
                runProxyButton.Text = "Connect";
                runProxyButton.Visible = true;
                stopProxyButton.Visible = false;
                hostAddressComboBox.Enabled = true;
                wc3VersionComboBox.Enabled = true;
                gameTypeComboBox.Enabled = true;
                rescanButton.Enabled = true;
                UpdateConnectButtonState();
                UpdateConnectionStatus("Not connected", Color.Gray);
                return;
            }

            Log($"Reconnecting to {_lastHostInfo.Hostname}...", LogLevel.Warning);
            if (RunProxy(_lastHostInfo))
            {
                _reconnectManager.OnReconnectSucceeded();
                Log("Reconnected successfully", LogLevel.Success);
            }
            else
            {
                _reconnectManager.OnReconnectFailed();
            }
        }

        private void ProxyService_GameFound(object sender, GameInfo gameInfo)
        {
            BeginInvoke(() =>
            {
                gamePortValueLabel.Text = gameInfo.Port.ToString();
                gameNameValueLabel.Text = gameInfo.Name;
                gameTypeValueLabel.Text = gameInfo.GameType.ToString();
                mapNameValueLabel.Text = gameInfo.MapName;
                mapSizeValueLabel.Text = $"{gameInfo.MapSizeCategory} ({gameInfo.MapHeight} x {gameInfo.MapWidth})";
                playersCountValueLabel.Text = $"{gameInfo.CurrentPlayersCount} / {gameInfo.PlayerSlotsCount} / {gameInfo.SlotsCount}";

                int latency = _proxyService?.LatencyMs ?? -1;
                string latencyText = latency >= 0 ? $" ({latency}ms)" : "";
                UpdateConnectionStatus($"\u25CF Game found{latencyText}", Color.LimeGreen);
            });
        }

        private void ProxyService_GameLost(object sender, EventArgs e)
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
                UpdateConnectionStatus("\u25CF Searching for game...", Color.FromArgb(255, 180, 0));
            });
        }

        private void ProxyService_Notification(object sender, string text)
        {
            Log(text);
            BeginInvoke(() => Notify(text));
        }

        private void ProxyService_ConnectionCountChanged(object sender, EventArgs e)
        {
            BeginInvoke(() =>
            {
                clientCountValueLabel.Text = (_proxyService?.ConnectionCount ?? 0).ToString();
            });
        }

        #endregion
    }
}
