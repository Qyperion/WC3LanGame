using System.Drawing.Drawing2D;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
        private readonly LogManager _log;
        private readonly ErrorProvider _hostAddressErrorProvider;

        private readonly Timer _updateWC3RunningStatusTimer = new(3000);

        // Reconnect state
        private readonly ReconnectManager _reconnectManager = new();
        private HostInfo _lastHostInfo;

        // Tray icon
        private Icon _originalTrayIcon;
        private Icon _currentStatusIcon;
        private ContextMenuStrip _trayContextMenu;
        private ToolStripMenuItem _trayShowHideItem;
        private ToolStripMenuItem _trayConnectItem;
        private ToolStripMenuItem _trayWC3Item;


        public MainForm()
        {
            InitializeComponent();
            _log = new LogManager(logRichTextBox, lastLogStatusLabel);
            _hostAddressErrorProvider = new ErrorProvider(components) { BlinkStyle = ErrorBlinkStyle.NeverBlink };
            _settings = AppSettings.Load();
            InitSettingsComponent();
            InitTrayIcon();
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

        private void InitTrayIcon()
        {
            _originalTrayIcon = (Icon)notifyIcon.Icon.Clone();

            _trayShowHideItem = new ToolStripMenuItem("Show");
            _trayShowHideItem.Click += (_, _) => TrayMenu_ToggleWindow();

            _trayConnectItem = new ToolStripMenuItem("Connect");
            _trayConnectItem.Click += (_, _) => TrayMenu_ToggleProxy();

            _trayWC3Item = new ToolStripMenuItem("Run WC3");
            _trayWC3Item.Click += (_, _) => TrayMenu_ToggleWC3();

            var exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (_, _) => Close();

            _trayContextMenu = new ContextMenuStrip();
            _trayContextMenu.Items.Add(_trayShowHideItem);
            _trayContextMenu.Items.Add(new ToolStripSeparator());
            _trayContextMenu.Items.Add(_trayConnectItem);
            _trayContextMenu.Items.Add(_trayWC3Item);
            _trayContextMenu.Items.Add(new ToolStripSeparator());
            _trayContextMenu.Items.Add(exitItem);
            _trayContextMenu.Opening += TrayContextMenu_Opening;
            DarkMode.Apply(_trayContextMenu);

            notifyIcon.ContextMenuStrip = _trayContextMenu;
        }

        private void TrayContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool isVisible = WindowState != FormWindowState.Minimized && Visible;
            _trayShowHideItem.Text = isVisible ? "Hide" : "Show";

            bool proxyRunning = _proxyService?.IsRunning == true;
            _trayConnectItem.Text = proxyRunning ? "Disconnect" : "Connect";
            _trayConnectItem.Enabled = proxyRunning || runProxyButton.Enabled;

            bool wc3Running = WarcraftExecutable.IsWC3ProcessRunning(_settings.WarcraftExecutablePath);
            _trayWC3Item.Text = wc3Running ? "Stop WC3" : "Run WC3";
        }

        private void TrayMenu_ToggleWindow()
        {
            if (WindowState == FormWindowState.Minimized || !Visible)
            {
                Show();
                WindowState = FormWindowState.Normal;
                ShowInTaskbar = true;
                Focus();
            }
            else
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        private void TrayMenu_ToggleProxy()
        {
            if (_proxyService?.IsRunning == true)
                stopProxyButton_Click(this, EventArgs.Empty);
            else
                runProxyButton_Click(this, EventArgs.Empty);
        }

        private void TrayMenu_ToggleWC3()
        {
            bool wc3Running = WarcraftExecutable.IsWC3ProcessRunning(_settings.WarcraftExecutablePath);
            if (wc3Running)
                stopWC3Button_Click(this, EventArgs.Empty);
            else
                runWC3Button_Click(this, EventArgs.Empty);
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
            _currentStatusIcon?.Dispose();
            _trayContextMenu?.Dispose();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            Focus();
        }

        private void Notify(string text, ToolTipIcon icon = ToolTipIcon.Info)
        {
            notifyIcon.ShowBalloonTip(2000, "WC3 Proxy", text, icon);
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
                    _log.Log("Warcraft III executable was not selected.", LogLevel.Warning);
                    return;
                }

                _settings.WarcraftExecutablePath = executablePath;
                _settings.Save();
                TrySelectInstalledVersion(executablePath);
            }

            string message = WarcraftExecutable.RunWC3(executablePath);
            _log.Log(message);
            Notify(message);
        }

        private void stopWC3Button_Click(object sender, EventArgs e)
        {
            string result = WarcraftExecutable.StopWC3ProcessRunning(_settings.WarcraftExecutablePath);
            _log.Log(result);
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

                _log.Log($"Network scan complete, found {ipList.Length} active hosts");
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
            if (WindowState == FormWindowState.Minimized)
            {
                ShowInTaskbar = false;
            }
            else
            {
                ShowInTaskbar = true;
            }
        }

        private void UpdateConnectButtonState()
        {
            string text = hostAddressComboBox.Text.Trim();
            bool valid = !string.IsNullOrWhiteSpace(text) &&
                (IPAddress.TryParse(text, out _) ||
                 Uri.CheckHostName(text) != UriHostNameType.Unknown);

            runProxyButton.Enabled = valid;

            if (string.IsNullOrWhiteSpace(text))
                _hostAddressErrorProvider.SetError(hostAddressComboBox, "");
            else if (!valid)
                _hostAddressErrorProvider.SetError(hostAddressComboBox, "Enter a valid IP address or hostname");
            else
                _hostAddressErrorProvider.SetError(hostAddressComboBox, "");
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
            UpdateTrayIconStatus(color == Color.Gray ? Color.Empty : color);
        }

        private void UpdateTrayText()
        {
            string text;
            int count = _proxyService?.ConnectionCount ?? 0;
            string gameName = _proxyService?.CurrentGame?.Name;

            if (!string.IsNullOrEmpty(gameName))
                text = $"WC3 Proxy \u2014 {gameName}" + (count > 0 ? $" ({count})" : "");
            else if (_proxyService?.IsRunning == true)
                text = "WC3 Proxy \u2014 Searching...";
            else
                text = "WC3 Lan Game";

            // NotifyIcon.Text has a 63 character limit
            notifyIcon.Text = text.Length > 63 ? text[..63] : text;
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
                _log.Log($"Unable to start listener: {ex.Message}", LogLevel.Error);
                if (!_reconnectManager.IsReconnecting)
                    MessageBox.Show("Unable to start listener\n" + ex.Message);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                _proxyService.Dispose();
                _proxyService = null;
                _log.Log($"Error: {ex.Message}", LogLevel.Error);
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
            _log.Log($"Proxy started, connecting to {description}", LogLevel.Success);

            runProxyButton.Visible = false;
            stopProxyButton.Visible = true;
            gameInfoTableLayoutPanel.Visible = true;
            hostAddressComboBox.Enabled = false;
            wc3VersionComboBox.Enabled = false;
            gameTypeComboBox.Enabled = false;
            rescanButton.Enabled = false;
            UpdateConnectionStatus("\u25CF Searching for game...", Color.FromArgb(255, 180, 0));
            UpdateTrayText();
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
                _log.Log("Proxy stopped");
                ResetUIToDisconnected();
            }
        }

        private void ResetUIToDisconnected()
        {
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
            UpdateTrayText();
        }

        private void ProxyService_Faulted(object sender, EventArgs e)
        {
            BeginInvoke(() =>
            {
                if (_reconnectManager.IsReconnecting) return;

                Notify("Connection lost", ToolTipIcon.Warning);

                if (!autoReconnectCheckBox.Checked)
                {
                    _log.Log("Proxy connection lost", LogLevel.Error);
                    StopProxy();
                    return;
                }

                _reconnectManager.Start();
            });
        }

        private void ReconnectManager_ReconnectScheduled(object sender, ReconnectScheduledEventArgs e)
        {
            BeginInvoke(() =>
            {
                _log.Log($"Connection lost. Reconnecting in {e.DelaySeconds}s (attempt {e.Attempt})...", LogLevel.Warning);
                UpdateConnectionStatus($"\u25CF Reconnecting ({e.Attempt})...", Color.FromArgb(255, 180, 0));
            });
        }

        private void ReconnectManager_ReconnectRequested(object sender, EventArgs e)
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
                _log.Log("Reconnection cancelled", LogLevel.Warning);
                ResetUIToDisconnected();
                return;
            }

            _log.Log($"Reconnecting to {_lastHostInfo.Hostname}...", LogLevel.Warning);
            if (RunProxy(_lastHostInfo))
            {
                _reconnectManager.OnReconnectSucceeded();
                _log.Log("Reconnected successfully", LogLevel.Success);
                Notify("Reconnected successfully");
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
                bool isNewGame = gameNameValueLabel.Text == "-";

                gamePortValueLabel.Text = gameInfo.Port.ToString();
                gameNameValueLabel.Text = gameInfo.Name;
                gameTypeValueLabel.Text = gameInfo.GameType.ToString();
                mapNameValueLabel.Text = gameInfo.MapName;
                mapSizeValueLabel.Text = $"{gameInfo.MapSizeCategory} ({gameInfo.MapHeight} x {gameInfo.MapWidth})";
                playersCountValueLabel.Text = $"{gameInfo.CurrentPlayersCount} / {gameInfo.PlayerSlotsCount} / {gameInfo.SlotsCount}";

                int latency = _proxyService?.LatencyMs ?? -1;
                string latencyText = latency >= 0 ? $" ({latency}ms)" : "";
                UpdateConnectionStatus($"\u25CF Game found{latencyText}", Color.LimeGreen);
                UpdateTrayText();

                if (isNewGame)
                    Notify($"Game found: {gameInfo.Name}");
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
                UpdateTrayText();
            });
        }

        private void ProxyService_Notification(object sender, string text)
        {
            _log.Log(text);
            BeginInvoke(() => Notify(text));
        }

        private void ProxyService_ConnectionCountChanged(object sender, EventArgs e)
        {
            BeginInvoke(() =>
            {
                clientCountValueLabel.Text = (_proxyService?.ConnectionCount ?? 0).ToString();
                UpdateTrayText();
            });
        }

        [DllImport("user32.dll", SetLastError = false)]
        private static extern bool DestroyIcon(IntPtr handle);

        private void UpdateTrayIconStatus(Color dotColor)
        {
            _currentStatusIcon?.Dispose();
            _currentStatusIcon = null;

            if (dotColor.IsEmpty || _originalTrayIcon == null)
            {
                notifyIcon.Icon = _originalTrayIcon;
                return;
            }

            _currentStatusIcon = CreateOverlayIcon(_originalTrayIcon, dotColor);
            notifyIcon.Icon = _currentStatusIcon;
        }

        private static Icon CreateOverlayIcon(Icon baseIcon, Color dotColor)
        {
            using Bitmap bmp = baseIcon.ToBitmap();
            using Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int dotSize = Math.Max(bmp.Width / 3, 4);
            int x = bmp.Width - dotSize - 1;
            int y = bmp.Height - dotSize - 1;

            using (var borderBrush = new SolidBrush(Color.FromArgb(30, 30, 30)))
                g.FillEllipse(borderBrush, x - 1, y - 1, dotSize + 2, dotSize + 2);
            using (var dotBrush = new SolidBrush(dotColor))
                g.FillEllipse(dotBrush, x, y, dotSize, dotSize);

            IntPtr hIcon = bmp.GetHicon();
            try
            {
                using var tempIcon = Icon.FromHandle(hIcon);
                return (Icon)tempIcon.Clone();
            }
            finally
            {
                DestroyIcon(hIcon);
            }
        }

        #endregion
    }
}
