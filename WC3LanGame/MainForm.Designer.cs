namespace WC3LanGame
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            toolTip = new ToolTip(components);
            autoReconnectCheckBox = new CheckBox();
            stopWC3Button = new Button();
            scanningNetworkProgressBar = new ProgressBar();
            runProxyButton = new Button();
            stopProxyButton = new Button();
            runWC3Button = new Button();
            hostAddressComboBox = new ComboBox();
            rescanButton = new Button();
            settingsPanel = new OverlayPanel();
            hostLabel = new Label();
            scanningNetworkLabel = new Label();
            versionLabel = new Label();
            wc3VersionComboBox = new ComboBox();
            gameLabel = new Label();
            gameTypeComboBox = new ComboBox();
            wc3ProcessRunningStatusLabel = new Label();
            gameInfoPanel = new OverlayPanel();
            connectionStatusLabel = new Label();
            gameInfoTableLayoutPanel = new TableLayoutPanel();
            hostAddressTitleLabel = new Label();
            hostAddressValueLabel = new Label();
            gamePortTitleLabel = new Label();
            gamePortValueLabel = new Label();
            gameTypeTitleLabel = new Label();
            gameTypeValueLabel = new Label();
            gameNameTitleLabel = new Label();
            gameNameValueLabel = new Label();
            mapNameTitleLabel = new Label();
            mapNameValueLabel = new Label();
            mapSizeTitleLabel = new Label();
            mapSizeValueLabel = new Label();
            playersCountTitleLabel = new Label();
            playersCountValueLabel = new Label();
            clientCountTitleLabel = new Label();
            clientCountValueLabel = new Label();
            logPanel = new Panel();
            logRichTextBox = new RichTextBox();
            statusStrip = new StatusStrip();
            lastLogStatusLabel = new ToolStripStatusLabel();
            showLogToolStripLabel = new ToolStripStatusLabel();
            notifyIcon = new NotifyIcon(components);
            settingsPanel.SuspendLayout();
            gameInfoPanel.SuspendLayout();
            gameInfoTableLayoutPanel.SuspendLayout();
            logPanel.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // autoReconnectCheckBox
            // 
            autoReconnectCheckBox.AutoSize = true;
            autoReconnectCheckBox.BackColor = Color.Transparent;
            autoReconnectCheckBox.Font = new Font("Segoe UI", 10F);
            autoReconnectCheckBox.ForeColor = Color.White;
            autoReconnectCheckBox.Location = new Point(114, 327);
            autoReconnectCheckBox.Margin = new Padding(4, 5, 4, 5);
            autoReconnectCheckBox.Name = "autoReconnectCheckBox";
            autoReconnectCheckBox.Size = new Size(175, 32);
            autoReconnectCheckBox.TabIndex = 11;
            autoReconnectCheckBox.Text = "Auto-reconnect";
            toolTip.SetToolTip(autoReconnectCheckBox, "Automatically reconnect when the connection is lost");
            autoReconnectCheckBox.UseVisualStyleBackColor = false;
            // 
            // stopWC3Button
            // 
            stopWC3Button.Font = new Font("Segoe UI", 12F);
            stopWC3Button.Location = new Point(286, 375);
            stopWC3Button.Margin = new Padding(4, 5, 4, 5);
            stopWC3Button.Name = "stopWC3Button";
            stopWC3Button.Size = new Size(129, 50);
            stopWC3Button.TabIndex = 14;
            stopWC3Button.Text = "Stop WC3";
            toolTip.SetToolTip(stopWC3Button, "Force-stop all running Warcraft III processes");
            stopWC3Button.UseVisualStyleBackColor = true;
            stopWC3Button.Visible = false;
            stopWC3Button.Click += stopWC3Button_Click;
            // 
            // scanningNetworkProgressBar
            // 
            scanningNetworkProgressBar.Location = new Point(193, 82);
            scanningNetworkProgressBar.Margin = new Padding(4, 5, 4, 5);
            scanningNetworkProgressBar.Name = "scanningNetworkProgressBar";
            scanningNetworkProgressBar.Size = new Size(221, 28);
            scanningNetworkProgressBar.TabIndex = 4;
            toolTip.SetToolTip(scanningNetworkProgressBar, "Scanning local networks for active hosts");
            // 
            // runProxyButton
            // 
            runProxyButton.Font = new Font("Segoe UI", 16F);
            runProxyButton.Location = new Point(114, 253);
            runProxyButton.Margin = new Padding(4, 5, 4, 5);
            runProxyButton.Name = "runProxyButton";
            runProxyButton.Size = new Size(300, 63);
            runProxyButton.TabIndex = 9;
            runProxyButton.Text = "Connect";
            toolTip.SetToolTip(runProxyButton, "Connect to the host and start game discovery");
            runProxyButton.UseVisualStyleBackColor = true;
            runProxyButton.Click += runProxyButton_Click;
            // 
            // stopProxyButton
            // 
            stopProxyButton.Font = new Font("Segoe UI", 16F);
            stopProxyButton.Location = new Point(114, 253);
            stopProxyButton.Margin = new Padding(4, 5, 4, 5);
            stopProxyButton.Name = "stopProxyButton";
            stopProxyButton.Size = new Size(300, 63);
            stopProxyButton.TabIndex = 10;
            stopProxyButton.Text = "Stop";
            toolTip.SetToolTip(stopProxyButton, "Disconnect from host and stop game discovery");
            stopProxyButton.UseVisualStyleBackColor = true;
            stopProxyButton.Visible = false;
            stopProxyButton.Click += stopProxyButton_Click;
            // 
            // runWC3Button
            // 
            runWC3Button.Font = new Font("Segoe UI", 12F);
            runWC3Button.Location = new Point(286, 375);
            runWC3Button.Margin = new Padding(4, 5, 4, 5);
            runWC3Button.Name = "runWC3Button";
            runWC3Button.Size = new Size(129, 50);
            runWC3Button.TabIndex = 13;
            runWC3Button.Text = "Run WC3";
            toolTip.SetToolTip(runWC3Button, "Launch Warcraft III");
            runWC3Button.UseVisualStyleBackColor = true;
            runWC3Button.Click += runWC3Button_Click;
            // 
            // hostAddressComboBox
            // 
            hostAddressComboBox.Font = new Font("Segoe UI", 14F);
            hostAddressComboBox.FormattingEnabled = true;
            hostAddressComboBox.Location = new Point(114, 17);
            hostAddressComboBox.Margin = new Padding(4, 5, 4, 5);
            hostAddressComboBox.Name = "hostAddressComboBox";
            hostAddressComboBox.Size = new Size(234, 46);
            hostAddressComboBox.TabIndex = 1;
            hostAddressComboBox.Text = "10.8.0.14";
            toolTip.SetToolTip(hostAddressComboBox, "IP address or hostname of the game host");
            // 
            // rescanButton
            // 
            rescanButton.Font = new Font("Segoe UI", 9F);
            rescanButton.Location = new Point(360, 16);
            rescanButton.Margin = new Padding(4, 5, 4, 5);
            rescanButton.Name = "rescanButton";
            rescanButton.Size = new Size(54, 50);
            rescanButton.TabIndex = 2;
            rescanButton.Text = "Scan";
            toolTip.SetToolTip(rescanButton, "Rescan local networks for active hosts");
            rescanButton.UseVisualStyleBackColor = true;
            rescanButton.Click += rescanButton_Click;
            // 
            // settingsPanel
            // 
            settingsPanel.BackColor = Color.Transparent;
            settingsPanel.Controls.Add(hostLabel);
            settingsPanel.Controls.Add(hostAddressComboBox);
            settingsPanel.Controls.Add(rescanButton);
            settingsPanel.Controls.Add(scanningNetworkLabel);
            settingsPanel.Controls.Add(scanningNetworkProgressBar);
            settingsPanel.Controls.Add(versionLabel);
            settingsPanel.Controls.Add(wc3VersionComboBox);
            settingsPanel.Controls.Add(gameLabel);
            settingsPanel.Controls.Add(gameTypeComboBox);
            settingsPanel.Controls.Add(runProxyButton);
            settingsPanel.Controls.Add(stopProxyButton);
            settingsPanel.Controls.Add(autoReconnectCheckBox);
            settingsPanel.Controls.Add(wc3ProcessRunningStatusLabel);
            settingsPanel.Controls.Add(runWC3Button);
            settingsPanel.Controls.Add(stopWC3Button);
            settingsPanel.Location = new Point(11, 13);
            settingsPanel.Margin = new Padding(4, 5, 4, 5);
            settingsPanel.Name = "settingsPanel";
            settingsPanel.Size = new Size(429, 447);
            settingsPanel.TabIndex = 0;
            // 
            // hostLabel
            // 
            hostLabel.AutoSize = true;
            hostLabel.BackColor = Color.Transparent;
            hostLabel.Font = new Font("Segoe UI", 14F);
            hostLabel.ForeColor = Color.White;
            hostLabel.Location = new Point(14, 23);
            hostLabel.Margin = new Padding(4, 0, 4, 0);
            hostLabel.Name = "hostLabel";
            hostLabel.Size = new Size(80, 38);
            hostLabel.TabIndex = 0;
            hostLabel.Text = "Host:";
            // 
            // scanningNetworkLabel
            // 
            scanningNetworkLabel.AutoSize = true;
            scanningNetworkLabel.BackColor = Color.Transparent;
            scanningNetworkLabel.Font = new Font("Segoe UI", 10F);
            scanningNetworkLabel.ForeColor = Color.White;
            scanningNetworkLabel.Location = new Point(14, 78);
            scanningNetworkLabel.Margin = new Padding(4, 0, 4, 0);
            scanningNetworkLabel.Name = "scanningNetworkLabel";
            scanningNetworkLabel.Size = new Size(173, 28);
            scanningNetworkLabel.TabIndex = 3;
            scanningNetworkLabel.Text = "Network scanning:";
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.BackColor = Color.Transparent;
            versionLabel.Font = new Font("Segoe UI", 14F);
            versionLabel.ForeColor = Color.White;
            versionLabel.Location = new Point(14, 123);
            versionLabel.Margin = new Padding(4, 0, 4, 0);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(114, 38);
            versionLabel.TabIndex = 5;
            versionLabel.Text = "Version:";
            // 
            // wc3VersionComboBox
            // 
            wc3VersionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            wc3VersionComboBox.Font = new Font("Segoe UI", 14F);
            wc3VersionComboBox.FormattingEnabled = true;
            wc3VersionComboBox.Location = new Point(160, 117);
            wc3VersionComboBox.Margin = new Padding(4, 5, 4, 5);
            wc3VersionComboBox.Name = "wc3VersionComboBox";
            wc3VersionComboBox.Size = new Size(253, 46);
            wc3VersionComboBox.TabIndex = 6;
            // 
            // gameLabel
            // 
            gameLabel.AutoSize = true;
            gameLabel.BackColor = Color.Transparent;
            gameLabel.Font = new Font("Segoe UI", 14F);
            gameLabel.ForeColor = Color.White;
            gameLabel.Location = new Point(14, 190);
            gameLabel.Margin = new Padding(4, 0, 4, 0);
            gameLabel.Name = "gameLabel";
            gameLabel.Size = new Size(95, 38);
            gameLabel.TabIndex = 7;
            gameLabel.Text = "Game:";
            // 
            // gameTypeComboBox
            // 
            gameTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            gameTypeComboBox.Font = new Font("Segoe UI", 14F);
            gameTypeComboBox.FormattingEnabled = true;
            gameTypeComboBox.Location = new Point(114, 183);
            gameTypeComboBox.Margin = new Padding(4, 5, 4, 5);
            gameTypeComboBox.Name = "gameTypeComboBox";
            gameTypeComboBox.Size = new Size(298, 46);
            gameTypeComboBox.TabIndex = 8;
            // 
            // wc3ProcessRunningStatusLabel
            // 
            wc3ProcessRunningStatusLabel.AutoSize = true;
            wc3ProcessRunningStatusLabel.BackColor = Color.Transparent;
            wc3ProcessRunningStatusLabel.Font = new Font("Segoe UI", 12F);
            wc3ProcessRunningStatusLabel.ForeColor = Color.White;
            wc3ProcessRunningStatusLabel.Location = new Point(14, 380);
            wc3ProcessRunningStatusLabel.Margin = new Padding(4, 0, 4, 0);
            wc3ProcessRunningStatusLabel.Name = "wc3ProcessRunningStatusLabel";
            wc3ProcessRunningStatusLabel.Size = new Size(87, 32);
            wc3ProcessRunningStatusLabel.TabIndex = 12;
            wc3ProcessRunningStatusLabel.Text = "WC3 is";
            // 
            // gameInfoPanel
            // 
            gameInfoPanel.BackColor = Color.Transparent;
            gameInfoPanel.Controls.Add(connectionStatusLabel);
            gameInfoPanel.Controls.Add(gameInfoTableLayoutPanel);
            gameInfoPanel.Location = new Point(451, 13);
            gameInfoPanel.Margin = new Padding(4, 5, 4, 5);
            gameInfoPanel.Name = "gameInfoPanel";
            gameInfoPanel.Size = new Size(429, 447);
            gameInfoPanel.TabIndex = 1;
            // 
            // connectionStatusLabel
            // 
            connectionStatusLabel.AutoSize = true;
            connectionStatusLabel.BackColor = Color.Transparent;
            connectionStatusLabel.Font = new Font("Segoe UI", 11F);
            connectionStatusLabel.ForeColor = Color.Gray;
            connectionStatusLabel.Location = new Point(14, 13);
            connectionStatusLabel.Margin = new Padding(4, 0, 4, 0);
            connectionStatusLabel.Name = "connectionStatusLabel";
            connectionStatusLabel.Size = new Size(156, 30);
            connectionStatusLabel.TabIndex = 0;
            connectionStatusLabel.Text = "Not connected";
            // 
            // gameInfoTableLayoutPanel
            // 
            gameInfoTableLayoutPanel.BackColor = Color.Transparent;
            gameInfoTableLayoutPanel.ColumnCount = 2;
            gameInfoTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 143F));
            gameInfoTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            gameInfoTableLayoutPanel.Controls.Add(hostAddressTitleLabel, 0, 0);
            gameInfoTableLayoutPanel.Controls.Add(hostAddressValueLabel, 1, 0);
            gameInfoTableLayoutPanel.Controls.Add(gamePortTitleLabel, 0, 1);
            gameInfoTableLayoutPanel.Controls.Add(gamePortValueLabel, 1, 1);
            gameInfoTableLayoutPanel.Controls.Add(gameTypeTitleLabel, 0, 2);
            gameInfoTableLayoutPanel.Controls.Add(gameTypeValueLabel, 1, 2);
            gameInfoTableLayoutPanel.Controls.Add(gameNameTitleLabel, 0, 3);
            gameInfoTableLayoutPanel.Controls.Add(gameNameValueLabel, 1, 3);
            gameInfoTableLayoutPanel.Controls.Add(mapNameTitleLabel, 0, 4);
            gameInfoTableLayoutPanel.Controls.Add(mapNameValueLabel, 1, 4);
            gameInfoTableLayoutPanel.Controls.Add(mapSizeTitleLabel, 0, 5);
            gameInfoTableLayoutPanel.Controls.Add(mapSizeValueLabel, 1, 5);
            gameInfoTableLayoutPanel.Controls.Add(playersCountTitleLabel, 0, 6);
            gameInfoTableLayoutPanel.Controls.Add(playersCountValueLabel, 1, 6);
            gameInfoTableLayoutPanel.Controls.Add(clientCountTitleLabel, 0, 7);
            gameInfoTableLayoutPanel.Controls.Add(clientCountValueLabel, 1, 7);
            gameInfoTableLayoutPanel.Location = new Point(7, 53);
            gameInfoTableLayoutPanel.Margin = new Padding(4, 5, 4, 5);
            gameInfoTableLayoutPanel.Name = "gameInfoTableLayoutPanel";
            gameInfoTableLayoutPanel.RowCount = 8;
            gameInfoTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            gameInfoTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            gameInfoTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            gameInfoTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            gameInfoTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            gameInfoTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            gameInfoTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            gameInfoTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            gameInfoTableLayoutPanel.Size = new Size(414, 388);
            gameInfoTableLayoutPanel.TabIndex = 1;
            gameInfoTableLayoutPanel.Visible = false;
            // 
            // hostAddressTitleLabel
            // 
            hostAddressTitleLabel.AutoSize = true;
            hostAddressTitleLabel.BackColor = Color.Transparent;
            hostAddressTitleLabel.Font = new Font("Segoe UI", 11F);
            hostAddressTitleLabel.ForeColor = Color.FromArgb(180, 180, 180);
            hostAddressTitleLabel.Location = new Point(4, 0);
            hostAddressTitleLabel.Margin = new Padding(4, 0, 4, 0);
            hostAddressTitleLabel.Name = "hostAddressTitleLabel";
            hostAddressTitleLabel.Size = new Size(91, 48);
            hostAddressTitleLabel.TabIndex = 0;
            hostAddressTitleLabel.Text = "Host Address";
            // 
            // hostAddressValueLabel
            // 
            hostAddressValueLabel.AutoSize = true;
            hostAddressValueLabel.BackColor = Color.Transparent;
            hostAddressValueLabel.Font = new Font("Segoe UI", 11F);
            hostAddressValueLabel.ForeColor = Color.White;
            hostAddressValueLabel.Location = new Point(147, 0);
            hostAddressValueLabel.Margin = new Padding(4, 0, 4, 0);
            hostAddressValueLabel.Name = "hostAddressValueLabel";
            hostAddressValueLabel.Size = new Size(22, 30);
            hostAddressValueLabel.TabIndex = 1;
            hostAddressValueLabel.Text = "-";
            // 
            // gamePortTitleLabel
            // 
            gamePortTitleLabel.AutoSize = true;
            gamePortTitleLabel.BackColor = Color.Transparent;
            gamePortTitleLabel.Font = new Font("Segoe UI", 11F);
            gamePortTitleLabel.ForeColor = Color.FromArgb(180, 180, 180);
            gamePortTitleLabel.Location = new Point(4, 48);
            gamePortTitleLabel.Margin = new Padding(4, 0, 4, 0);
            gamePortTitleLabel.Name = "gamePortTitleLabel";
            gamePortTitleLabel.Size = new Size(115, 30);
            gamePortTitleLabel.TabIndex = 2;
            gamePortTitleLabel.Text = "Game Port";
            // 
            // gamePortValueLabel
            // 
            gamePortValueLabel.AutoSize = true;
            gamePortValueLabel.BackColor = Color.Transparent;
            gamePortValueLabel.Font = new Font("Segoe UI", 11F);
            gamePortValueLabel.ForeColor = Color.White;
            gamePortValueLabel.Location = new Point(147, 48);
            gamePortValueLabel.Margin = new Padding(4, 0, 4, 0);
            gamePortValueLabel.Name = "gamePortValueLabel";
            gamePortValueLabel.Size = new Size(22, 30);
            gamePortValueLabel.TabIndex = 3;
            gamePortValueLabel.Text = "-";
            // 
            // gameTypeTitleLabel
            // 
            gameTypeTitleLabel.AutoSize = true;
            gameTypeTitleLabel.BackColor = Color.Transparent;
            gameTypeTitleLabel.Font = new Font("Segoe UI", 11F);
            gameTypeTitleLabel.ForeColor = Color.FromArgb(180, 180, 180);
            gameTypeTitleLabel.Location = new Point(4, 96);
            gameTypeTitleLabel.Margin = new Padding(4, 0, 4, 0);
            gameTypeTitleLabel.Name = "gameTypeTitleLabel";
            gameTypeTitleLabel.Size = new Size(123, 30);
            gameTypeTitleLabel.TabIndex = 4;
            gameTypeTitleLabel.Text = "Game Type";
            // 
            // gameTypeValueLabel
            // 
            gameTypeValueLabel.AutoSize = true;
            gameTypeValueLabel.BackColor = Color.Transparent;
            gameTypeValueLabel.Font = new Font("Segoe UI", 11F);
            gameTypeValueLabel.ForeColor = Color.White;
            gameTypeValueLabel.Location = new Point(147, 96);
            gameTypeValueLabel.Margin = new Padding(4, 0, 4, 0);
            gameTypeValueLabel.Name = "gameTypeValueLabel";
            gameTypeValueLabel.Size = new Size(22, 30);
            gameTypeValueLabel.TabIndex = 5;
            gameTypeValueLabel.Text = "-";
            // 
            // gameNameTitleLabel
            // 
            gameNameTitleLabel.AutoSize = true;
            gameNameTitleLabel.BackColor = Color.Transparent;
            gameNameTitleLabel.Font = new Font("Segoe UI", 11F);
            gameNameTitleLabel.ForeColor = Color.FromArgb(180, 180, 180);
            gameNameTitleLabel.Location = new Point(4, 144);
            gameNameTitleLabel.Margin = new Padding(4, 0, 4, 0);
            gameNameTitleLabel.Name = "gameNameTitleLabel";
            gameNameTitleLabel.Size = new Size(134, 30);
            gameNameTitleLabel.TabIndex = 6;
            gameNameTitleLabel.Text = "Game Name";
            // 
            // gameNameValueLabel
            // 
            gameNameValueLabel.AutoSize = true;
            gameNameValueLabel.BackColor = Color.Transparent;
            gameNameValueLabel.Font = new Font("Segoe UI", 11F);
            gameNameValueLabel.ForeColor = Color.White;
            gameNameValueLabel.Location = new Point(147, 144);
            gameNameValueLabel.Margin = new Padding(4, 0, 4, 0);
            gameNameValueLabel.Name = "gameNameValueLabel";
            gameNameValueLabel.Size = new Size(22, 30);
            gameNameValueLabel.TabIndex = 7;
            gameNameValueLabel.Text = "-";
            // 
            // mapNameTitleLabel
            // 
            mapNameTitleLabel.AutoSize = true;
            mapNameTitleLabel.BackColor = Color.Transparent;
            mapNameTitleLabel.Font = new Font("Segoe UI", 11F);
            mapNameTitleLabel.ForeColor = Color.FromArgb(180, 180, 180);
            mapNameTitleLabel.Location = new Point(4, 192);
            mapNameTitleLabel.Margin = new Padding(4, 0, 4, 0);
            mapNameTitleLabel.Name = "mapNameTitleLabel";
            mapNameTitleLabel.Size = new Size(121, 30);
            mapNameTitleLabel.TabIndex = 8;
            mapNameTitleLabel.Text = "Map Name";
            // 
            // mapNameValueLabel
            // 
            mapNameValueLabel.AutoSize = true;
            mapNameValueLabel.BackColor = Color.Transparent;
            mapNameValueLabel.Font = new Font("Segoe UI", 11F);
            mapNameValueLabel.ForeColor = Color.White;
            mapNameValueLabel.Location = new Point(147, 192);
            mapNameValueLabel.Margin = new Padding(4, 0, 4, 0);
            mapNameValueLabel.Name = "mapNameValueLabel";
            mapNameValueLabel.Size = new Size(22, 30);
            mapNameValueLabel.TabIndex = 9;
            mapNameValueLabel.Text = "-";
            // 
            // mapSizeTitleLabel
            // 
            mapSizeTitleLabel.AutoSize = true;
            mapSizeTitleLabel.BackColor = Color.Transparent;
            mapSizeTitleLabel.Font = new Font("Segoe UI", 11F);
            mapSizeTitleLabel.ForeColor = Color.FromArgb(180, 180, 180);
            mapSizeTitleLabel.Location = new Point(4, 240);
            mapSizeTitleLabel.Margin = new Padding(4, 0, 4, 0);
            mapSizeTitleLabel.Name = "mapSizeTitleLabel";
            mapSizeTitleLabel.Size = new Size(102, 30);
            mapSizeTitleLabel.TabIndex = 10;
            mapSizeTitleLabel.Text = "Map Size";
            // 
            // mapSizeValueLabel
            // 
            mapSizeValueLabel.AutoSize = true;
            mapSizeValueLabel.BackColor = Color.Transparent;
            mapSizeValueLabel.Font = new Font("Segoe UI", 11F);
            mapSizeValueLabel.ForeColor = Color.White;
            mapSizeValueLabel.Location = new Point(147, 240);
            mapSizeValueLabel.Margin = new Padding(4, 0, 4, 0);
            mapSizeValueLabel.Name = "mapSizeValueLabel";
            mapSizeValueLabel.Size = new Size(22, 30);
            mapSizeValueLabel.TabIndex = 11;
            mapSizeValueLabel.Text = "-";
            // 
            // playersCountTitleLabel
            // 
            playersCountTitleLabel.AutoSize = true;
            playersCountTitleLabel.BackColor = Color.Transparent;
            playersCountTitleLabel.Font = new Font("Segoe UI", 11F);
            playersCountTitleLabel.ForeColor = Color.FromArgb(180, 180, 180);
            playersCountTitleLabel.Location = new Point(4, 288);
            playersCountTitleLabel.Margin = new Padding(4, 0, 4, 0);
            playersCountTitleLabel.Name = "playersCountTitleLabel";
            playersCountTitleLabel.Size = new Size(81, 30);
            playersCountTitleLabel.TabIndex = 12;
            playersCountTitleLabel.Text = "Players";
            // 
            // playersCountValueLabel
            // 
            playersCountValueLabel.AutoSize = true;
            playersCountValueLabel.BackColor = Color.Transparent;
            playersCountValueLabel.Font = new Font("Segoe UI", 11F);
            playersCountValueLabel.ForeColor = Color.White;
            playersCountValueLabel.Location = new Point(147, 288);
            playersCountValueLabel.Margin = new Padding(4, 0, 4, 0);
            playersCountValueLabel.Name = "playersCountValueLabel";
            playersCountValueLabel.Size = new Size(22, 30);
            playersCountValueLabel.TabIndex = 13;
            playersCountValueLabel.Text = "-";
            // 
            // clientCountTitleLabel
            // 
            clientCountTitleLabel.AutoSize = true;
            clientCountTitleLabel.BackColor = Color.Transparent;
            clientCountTitleLabel.Font = new Font("Segoe UI", 11F);
            clientCountTitleLabel.ForeColor = Color.FromArgb(180, 180, 180);
            clientCountTitleLabel.Location = new Point(4, 336);
            clientCountTitleLabel.Margin = new Padding(4, 0, 4, 0);
            clientCountTitleLabel.Name = "clientCountTitleLabel";
            clientCountTitleLabel.Size = new Size(77, 30);
            clientCountTitleLabel.TabIndex = 14;
            clientCountTitleLabel.Text = "Clients";
            // 
            // clientCountValueLabel
            // 
            clientCountValueLabel.AutoSize = true;
            clientCountValueLabel.BackColor = Color.Transparent;
            clientCountValueLabel.Font = new Font("Segoe UI", 11F);
            clientCountValueLabel.ForeColor = Color.White;
            clientCountValueLabel.Location = new Point(147, 336);
            clientCountValueLabel.Margin = new Padding(4, 0, 4, 0);
            clientCountValueLabel.Name = "clientCountValueLabel";
            clientCountValueLabel.Size = new Size(22, 30);
            clientCountValueLabel.TabIndex = 15;
            clientCountValueLabel.Text = "-";
            // 
            // logPanel
            // 
            logPanel.BackColor = Color.FromArgb(20, 20, 30);
            logPanel.Controls.Add(logRichTextBox);
            logPanel.Location = new Point(11, 473);
            logPanel.Margin = new Padding(4, 5, 4, 5);
            logPanel.Name = "logPanel";
            logPanel.Size = new Size(869, 245);
            logPanel.TabIndex = 2;
            logPanel.Visible = false;
            // 
            // logRichTextBox
            // 
            logRichTextBox.BackColor = Color.FromArgb(30, 30, 30);
            logRichTextBox.Font = new Font("Consolas", 9F);
            logRichTextBox.ForeColor = Color.FromArgb(220, 220, 220);
            logRichTextBox.Location = new Point(7, 8);
            logRichTextBox.Margin = new Padding(4, 5, 4, 5);
            logRichTextBox.Name = "logRichTextBox";
            logRichTextBox.ReadOnly = true;
            logRichTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            logRichTextBox.Size = new Size(853, 226);
            logRichTextBox.TabIndex = 0;
            logRichTextBox.Text = "";
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(24, 24);
            statusStrip.Items.AddRange(new ToolStripItem[] { lastLogStatusLabel, showLogToolStripLabel });
            statusStrip.Location = new Point(0, 485);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(1, 0, 20, 0);
            statusStrip.Size = new Size(891, 32);
            statusStrip.SizingGrip = false;
            statusStrip.TabIndex = 3;
            // 
            // lastLogStatusLabel
            // 
            lastLogStatusLabel.Name = "lastLogStatusLabel";
            lastLogStatusLabel.Size = new Size(758, 25);
            lastLogStatusLabel.Spring = true;
            lastLogStatusLabel.Text = "Ready";
            lastLogStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // showLogToolStripLabel
            // 
            showLogToolStripLabel.ActiveLinkColor = Color.White;
            showLogToolStripLabel.IsLink = true;
            showLogToolStripLabel.LinkColor = Color.FromArgb(180, 200, 255);
            showLogToolStripLabel.Name = "showLogToolStripLabel";
            showLogToolStripLabel.Size = new Size(112, 25);
            showLogToolStripLabel.Text = "Show Log ▼";
            showLogToolStripLabel.VisitedLinkColor = Color.FromArgb(180, 200, 255);
            showLogToolStripLabel.Click += showLogToolStripLabel_Click;
            // 
            // notifyIcon
            // 
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "WC3 Lan Game";
            notifyIcon.Visible = true;
            notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.WarcraftBackgroundImage;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(891, 517);
            Controls.Add(settingsPanel);
            Controls.Add(gameInfoPanel);
            Controls.Add(logPanel);
            Controls.Add(statusStrip);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            Name = "MainForm";
            Text = "WC3 Lan Game";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            Resize += MainForm_Resize;
            settingsPanel.ResumeLayout(false);
            settingsPanel.PerformLayout();
            gameInfoPanel.ResumeLayout(false);
            gameInfoPanel.PerformLayout();
            gameInfoTableLayoutPanel.ResumeLayout(false);
            gameInfoTableLayoutPanel.PerformLayout();
            logPanel.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OverlayPanel settingsPanel;
        private OverlayPanel gameInfoPanel;
        private Panel logPanel;
        private Label hostLabel;
        private ComboBox hostAddressComboBox;
        private Button rescanButton;
        private Label scanningNetworkLabel;
        private ProgressBar scanningNetworkProgressBar;
        private Label versionLabel;
        private ComboBox wc3VersionComboBox;
        private Label gameLabel;
        private ComboBox gameTypeComboBox;
        private Button runProxyButton;
        private Button stopProxyButton;
        private CheckBox autoReconnectCheckBox;
        private Label wc3ProcessRunningStatusLabel;
        private Button runWC3Button;
        private Button stopWC3Button;
        private Label connectionStatusLabel;
        private TableLayoutPanel gameInfoTableLayoutPanel;
        private Label hostAddressTitleLabel;
        private Label hostAddressValueLabel;
        private Label gamePortTitleLabel;
        private Label gamePortValueLabel;
        private Label gameTypeTitleLabel;
        private Label gameTypeValueLabel;
        private Label gameNameTitleLabel;
        private Label gameNameValueLabel;
        private Label mapNameTitleLabel;
        private Label mapNameValueLabel;
        private Label mapSizeTitleLabel;
        private Label mapSizeValueLabel;
        private Label playersCountTitleLabel;
        private Label playersCountValueLabel;
        private Label clientCountTitleLabel;
        private Label clientCountValueLabel;
        private RichTextBox logRichTextBox;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lastLogStatusLabel;
        private ToolStripStatusLabel showLogToolStripLabel;
        private NotifyIcon notifyIcon;
        private ToolTip toolTip;
    }
}
