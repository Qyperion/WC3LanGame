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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.wc3VersionComboBox = new System.Windows.Forms.ComboBox();
            this.gameTypeComboBox = new System.Windows.Forms.ComboBox();
            this.hostLabel = new System.Windows.Forms.Label();
            this.gameLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.runProxyButton = new System.Windows.Forms.Button();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.gameInfoTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.mapSizeValueLabel = new System.Windows.Forms.Label();
            this.mapSizeTitleLabel = new System.Windows.Forms.Label();
            this.gameTypeValueLabel = new System.Windows.Forms.Label();
            this.gameTypeTitleLabel = new System.Windows.Forms.Label();
            this.hostAddressValueLabel = new System.Windows.Forms.Label();
            this.gamePortTitleLabel = new System.Windows.Forms.Label();
            this.gamePortValueLabel = new System.Windows.Forms.Label();
            this.clientCountTitleLabel = new System.Windows.Forms.Label();
            this.clientCountValueLabel = new System.Windows.Forms.Label();
            this.playersCountTitleLabel = new System.Windows.Forms.Label();
            this.playersCountValueLabel = new System.Windows.Forms.Label();
            this.mapNameTitleLabel = new System.Windows.Forms.Label();
            this.mapNameValueLabel = new System.Windows.Forms.Label();
            this.gameNameTitleLabel = new System.Windows.Forms.Label();
            this.gameNameValueLabel = new System.Windows.Forms.Label();
            this.hostAddressTitleLabel = new System.Windows.Forms.Label();
            this.stopProxyButton = new System.Windows.Forms.Button();
            this.runWC3Button = new System.Windows.Forms.Button();
            this.wc3ProcessRunningStatusLabel = new System.Windows.Forms.Label();
            this.scanningNetworkProgressBar = new System.Windows.Forms.ProgressBar();
            this.hostAddressComboBox = new System.Windows.Forms.ComboBox();
            this.scanningNetworkLabel = new System.Windows.Forms.Label();
            this.stopWC3Button = new System.Windows.Forms.Button();
            this.proxyActiveLabel = new System.Windows.Forms.Label();
            this.gameInfoTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // wc3VersionComboBox
            // 
            this.wc3VersionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.wc3VersionComboBox.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.wc3VersionComboBox.FormattingEnabled = true;
            this.wc3VersionComboBox.Location = new System.Drawing.Point(101, 73);
            this.wc3VersionComboBox.Name = "wc3VersionComboBox";
            this.wc3VersionComboBox.Size = new System.Drawing.Size(172, 33);
            this.wc3VersionComboBox.TabIndex = 2;
            // 
            // gameTypeComboBox
            // 
            this.gameTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gameTypeComboBox.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gameTypeComboBox.FormattingEnabled = true;
            this.gameTypeComboBox.Location = new System.Drawing.Point(101, 130);
            this.gameTypeComboBox.Name = "gameTypeComboBox";
            this.gameTypeComboBox.Size = new System.Drawing.Size(172, 33);
            this.gameTypeComboBox.TabIndex = 3;
            // 
            // hostLabel
            // 
            this.hostLabel.AutoSize = true;
            this.hostLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.hostLabel.Location = new System.Drawing.Point(12, 12);
            this.hostLabel.Name = "hostLabel";
            this.hostLabel.Size = new System.Drawing.Size(54, 25);
            this.hostLabel.TabIndex = 5;
            this.hostLabel.Text = "Host:";
            // 
            // gameLabel
            // 
            this.gameLabel.AutoSize = true;
            this.gameLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gameLabel.Location = new System.Drawing.Point(12, 130);
            this.gameLabel.Name = "gameLabel";
            this.gameLabel.Size = new System.Drawing.Size(65, 25);
            this.gameLabel.TabIndex = 6;
            this.gameLabel.Text = "Game:";
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.versionLabel.Location = new System.Drawing.Point(12, 73);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(79, 25);
            this.versionLabel.TabIndex = 7;
            this.versionLabel.Text = "Version:";
            // 
            // runProxyButton
            // 
            this.runProxyButton.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.runProxyButton.Location = new System.Drawing.Point(153, 183);
            this.runProxyButton.Name = "runProxyButton";
            this.runProxyButton.Size = new System.Drawing.Size(120, 40);
            this.runProxyButton.TabIndex = 8;
            this.runProxyButton.Text = "Connect";
            this.runProxyButton.UseVisualStyleBackColor = true;
            this.runProxyButton.Click += new System.EventHandler(this.runProxyButton_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "WC3 Lan Game";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // gameInfoTableLayoutPanel
            // 
            this.gameInfoTableLayoutPanel.ColumnCount = 2;
            this.gameInfoTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.gameInfoTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.gameInfoTableLayoutPanel.Controls.Add(this.mapSizeValueLabel, 1, 5);
            this.gameInfoTableLayoutPanel.Controls.Add(this.mapSizeTitleLabel, 0, 5);
            this.gameInfoTableLayoutPanel.Controls.Add(this.gameTypeValueLabel, 1, 2);
            this.gameInfoTableLayoutPanel.Controls.Add(this.gameTypeTitleLabel, 0, 2);
            this.gameInfoTableLayoutPanel.Controls.Add(this.hostAddressValueLabel, 1, 0);
            this.gameInfoTableLayoutPanel.Controls.Add(this.gamePortTitleLabel, 0, 1);
            this.gameInfoTableLayoutPanel.Controls.Add(this.gamePortValueLabel, 1, 1);
            this.gameInfoTableLayoutPanel.Controls.Add(this.clientCountTitleLabel, 0, 7);
            this.gameInfoTableLayoutPanel.Controls.Add(this.clientCountValueLabel, 1, 7);
            this.gameInfoTableLayoutPanel.Controls.Add(this.playersCountTitleLabel, 0, 6);
            this.gameInfoTableLayoutPanel.Controls.Add(this.playersCountValueLabel, 1, 6);
            this.gameInfoTableLayoutPanel.Controls.Add(this.mapNameTitleLabel, 0, 4);
            this.gameInfoTableLayoutPanel.Controls.Add(this.mapNameValueLabel, 1, 4);
            this.gameInfoTableLayoutPanel.Controls.Add(this.gameNameTitleLabel, 0, 3);
            this.gameInfoTableLayoutPanel.Controls.Add(this.gameNameValueLabel, 1, 3);
            this.gameInfoTableLayoutPanel.Controls.Add(this.hostAddressTitleLabel, 0, 0);
            this.gameInfoTableLayoutPanel.Location = new System.Drawing.Point(322, 12);
            this.gameInfoTableLayoutPanel.Name = "gameInfoTableLayoutPanel";
            this.gameInfoTableLayoutPanel.RowCount = 8;
            this.gameInfoTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.gameInfoTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.gameInfoTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.gameInfoTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.gameInfoTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.gameInfoTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.gameInfoTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.gameInfoTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.gameInfoTableLayoutPanel.Size = new System.Drawing.Size(291, 246);
            this.gameInfoTableLayoutPanel.TabIndex = 9;
            this.gameInfoTableLayoutPanel.Visible = false;
            // 
            // mapSizeValueLabel
            // 
            this.mapSizeValueLabel.AutoSize = true;
            this.mapSizeValueLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.mapSizeValueLabel.Location = new System.Drawing.Point(123, 150);
            this.mapSizeValueLabel.Name = "mapSizeValueLabel";
            this.mapSizeValueLabel.Size = new System.Drawing.Size(16, 21);
            this.mapSizeValueLabel.TabIndex = 17;
            this.mapSizeValueLabel.Text = "-";
            // 
            // mapSizeTitleLabel
            // 
            this.mapSizeTitleLabel.AutoSize = true;
            this.mapSizeTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.mapSizeTitleLabel.Location = new System.Drawing.Point(3, 150);
            this.mapSizeTitleLabel.Name = "mapSizeTitleLabel";
            this.mapSizeTitleLabel.Size = new System.Drawing.Size(73, 21);
            this.mapSizeTitleLabel.TabIndex = 17;
            this.mapSizeTitleLabel.Text = "Map Size";
            // 
            // gameTypeValueLabel
            // 
            this.gameTypeValueLabel.AutoSize = true;
            this.gameTypeValueLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gameTypeValueLabel.Location = new System.Drawing.Point(123, 60);
            this.gameTypeValueLabel.Name = "gameTypeValueLabel";
            this.gameTypeValueLabel.Size = new System.Drawing.Size(16, 21);
            this.gameTypeValueLabel.TabIndex = 17;
            this.gameTypeValueLabel.Text = "-";
            // 
            // gameTypeTitleLabel
            // 
            this.gameTypeTitleLabel.AutoSize = true;
            this.gameTypeTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gameTypeTitleLabel.Location = new System.Drawing.Point(3, 60);
            this.gameTypeTitleLabel.Name = "gameTypeTitleLabel";
            this.gameTypeTitleLabel.Size = new System.Drawing.Size(87, 21);
            this.gameTypeTitleLabel.TabIndex = 12;
            this.gameTypeTitleLabel.Text = "Game Type";
            // 
            // hostAddressValueLabel
            // 
            this.hostAddressValueLabel.AutoSize = true;
            this.hostAddressValueLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.hostAddressValueLabel.Location = new System.Drawing.Point(123, 0);
            this.hostAddressValueLabel.Name = "hostAddressValueLabel";
            this.hostAddressValueLabel.Size = new System.Drawing.Size(16, 21);
            this.hostAddressValueLabel.TabIndex = 1;
            this.hostAddressValueLabel.Text = "-";
            // 
            // gamePortTitleLabel
            // 
            this.gamePortTitleLabel.AutoSize = true;
            this.gamePortTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gamePortTitleLabel.Location = new System.Drawing.Point(3, 30);
            this.gamePortTitleLabel.Name = "gamePortTitleLabel";
            this.gamePortTitleLabel.Size = new System.Drawing.Size(83, 21);
            this.gamePortTitleLabel.TabIndex = 2;
            this.gamePortTitleLabel.Text = "Game Port";
            // 
            // gamePortValueLabel
            // 
            this.gamePortValueLabel.AutoSize = true;
            this.gamePortValueLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gamePortValueLabel.Location = new System.Drawing.Point(123, 30);
            this.gamePortValueLabel.Name = "gamePortValueLabel";
            this.gamePortValueLabel.Size = new System.Drawing.Size(16, 21);
            this.gamePortValueLabel.TabIndex = 3;
            this.gamePortValueLabel.Text = "-";
            // 
            // clientCountTitleLabel
            // 
            this.clientCountTitleLabel.AutoSize = true;
            this.clientCountTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.clientCountTitleLabel.Location = new System.Drawing.Point(3, 210);
            this.clientCountTitleLabel.Name = "clientCountTitleLabel";
            this.clientCountTitleLabel.Size = new System.Drawing.Size(57, 21);
            this.clientCountTitleLabel.TabIndex = 10;
            this.clientCountTitleLabel.Text = "Clients";
            // 
            // clientCountValueLabel
            // 
            this.clientCountValueLabel.AutoSize = true;
            this.clientCountValueLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.clientCountValueLabel.Location = new System.Drawing.Point(123, 210);
            this.clientCountValueLabel.Name = "clientCountValueLabel";
            this.clientCountValueLabel.Size = new System.Drawing.Size(16, 21);
            this.clientCountValueLabel.TabIndex = 11;
            this.clientCountValueLabel.Text = "-";
            // 
            // playersCountTitleLabel
            // 
            this.playersCountTitleLabel.AutoSize = true;
            this.playersCountTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.playersCountTitleLabel.Location = new System.Drawing.Point(3, 180);
            this.playersCountTitleLabel.Name = "playersCountTitleLabel";
            this.playersCountTitleLabel.Size = new System.Drawing.Size(60, 21);
            this.playersCountTitleLabel.TabIndex = 8;
            this.playersCountTitleLabel.Text = "Players";
            // 
            // playersCountValueLabel
            // 
            this.playersCountValueLabel.AutoSize = true;
            this.playersCountValueLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.playersCountValueLabel.Location = new System.Drawing.Point(123, 180);
            this.playersCountValueLabel.Name = "playersCountValueLabel";
            this.playersCountValueLabel.Size = new System.Drawing.Size(16, 21);
            this.playersCountValueLabel.TabIndex = 9;
            this.playersCountValueLabel.Text = "-";
            // 
            // mapNameTitleLabel
            // 
            this.mapNameTitleLabel.AutoSize = true;
            this.mapNameTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.mapNameTitleLabel.Location = new System.Drawing.Point(3, 120);
            this.mapNameTitleLabel.Name = "mapNameTitleLabel";
            this.mapNameTitleLabel.Size = new System.Drawing.Size(87, 21);
            this.mapNameTitleLabel.TabIndex = 6;
            this.mapNameTitleLabel.Text = "Map Name";
            // 
            // mapNameValueLabel
            // 
            this.mapNameValueLabel.AutoSize = true;
            this.mapNameValueLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.mapNameValueLabel.Location = new System.Drawing.Point(123, 120);
            this.mapNameValueLabel.Name = "mapNameValueLabel";
            this.mapNameValueLabel.Size = new System.Drawing.Size(16, 21);
            this.mapNameValueLabel.TabIndex = 7;
            this.mapNameValueLabel.Text = "-";
            // 
            // gameNameTitleLabel
            // 
            this.gameNameTitleLabel.AutoSize = true;
            this.gameNameTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gameNameTitleLabel.Location = new System.Drawing.Point(3, 90);
            this.gameNameTitleLabel.Name = "gameNameTitleLabel";
            this.gameNameTitleLabel.Size = new System.Drawing.Size(97, 21);
            this.gameNameTitleLabel.TabIndex = 4;
            this.gameNameTitleLabel.Text = "Game Name";
            // 
            // gameNameValueLabel
            // 
            this.gameNameValueLabel.AutoSize = true;
            this.gameNameValueLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gameNameValueLabel.Location = new System.Drawing.Point(123, 90);
            this.gameNameValueLabel.Name = "gameNameValueLabel";
            this.gameNameValueLabel.Size = new System.Drawing.Size(16, 21);
            this.gameNameValueLabel.TabIndex = 5;
            this.gameNameValueLabel.Text = "-";
            // 
            // hostAddressTitleLabel
            // 
            this.hostAddressTitleLabel.AutoSize = true;
            this.hostAddressTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.hostAddressTitleLabel.Location = new System.Drawing.Point(3, 0);
            this.hostAddressTitleLabel.Name = "hostAddressTitleLabel";
            this.hostAddressTitleLabel.Size = new System.Drawing.Size(102, 21);
            this.hostAddressTitleLabel.TabIndex = 0;
            this.hostAddressTitleLabel.Text = "Host Address";
            // 
            // stopProxyButton
            // 
            this.stopProxyButton.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.stopProxyButton.Location = new System.Drawing.Point(12, 183);
            this.stopProxyButton.Name = "stopProxyButton";
            this.stopProxyButton.Size = new System.Drawing.Size(120, 40);
            this.stopProxyButton.TabIndex = 10;
            this.stopProxyButton.Text = "Stop";
            this.stopProxyButton.UseVisualStyleBackColor = true;
            this.stopProxyButton.Visible = false;
            this.stopProxyButton.Click += new System.EventHandler(this.stopProxyButton_Click);
            // 
            // runWC3Button
            // 
            this.runWC3Button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.runWC3Button.Location = new System.Drawing.Point(159, 267);
            this.runWC3Button.Name = "runWC3Button";
            this.runWC3Button.Size = new System.Drawing.Size(91, 30);
            this.runWC3Button.TabIndex = 11;
            this.runWC3Button.Text = "Run WC3";
            this.runWC3Button.UseVisualStyleBackColor = true;
            this.runWC3Button.Click += new System.EventHandler(this.runWC3Button_Click);
            // 
            // wc3ProcessRunningStatusLabel
            // 
            this.wc3ProcessRunningStatusLabel.AutoSize = true;
            this.wc3ProcessRunningStatusLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.wc3ProcessRunningStatusLabel.Location = new System.Drawing.Point(12, 272);
            this.wc3ProcessRunningStatusLabel.Name = "wc3ProcessRunningStatusLabel";
            this.wc3ProcessRunningStatusLabel.Size = new System.Drawing.Size(59, 21);
            this.wc3ProcessRunningStatusLabel.TabIndex = 12;
            this.wc3ProcessRunningStatusLabel.Text = "WC3 is";
            // 
            // scanningNetworkProgressBar
            // 
            this.scanningNetworkProgressBar.Location = new System.Drawing.Point(138, 49);
            this.scanningNetworkProgressBar.Name = "scanningNetworkProgressBar";
            this.scanningNetworkProgressBar.Size = new System.Drawing.Size(135, 19);
            this.scanningNetworkProgressBar.TabIndex = 13;
            // 
            // hostAddressComboBox
            // 
            this.hostAddressComboBox.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.hostAddressComboBox.FormattingEnabled = true;
            this.hostAddressComboBox.Location = new System.Drawing.Point(101, 12);
            this.hostAddressComboBox.Name = "hostAddressComboBox";
            this.hostAddressComboBox.Size = new System.Drawing.Size(172, 33);
            this.hostAddressComboBox.TabIndex = 14;
            this.hostAddressComboBox.Text = "10.8.0.14";
            // 
            // scanningNetworkLabel
            // 
            this.scanningNetworkLabel.AutoSize = true;
            this.scanningNetworkLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.scanningNetworkLabel.Location = new System.Drawing.Point(12, 48);
            this.scanningNetworkLabel.Name = "scanningNetworkLabel";
            this.scanningNetworkLabel.Size = new System.Drawing.Size(122, 19);
            this.scanningNetworkLabel.TabIndex = 15;
            this.scanningNetworkLabel.Text = "Network scanning:";
            // 
            // stopWC3Button
            // 
            this.stopWC3Button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.stopWC3Button.Location = new System.Drawing.Point(159, 267);
            this.stopWC3Button.Name = "stopWC3Button";
            this.stopWC3Button.Size = new System.Drawing.Size(91, 30);
            this.stopWC3Button.TabIndex = 16;
            this.stopWC3Button.Text = "Stop WC3";
            this.stopWC3Button.UseVisualStyleBackColor = true;
            this.stopWC3Button.Visible = false;
            this.stopWC3Button.Click += new System.EventHandler(this.stopWC3Button_Click);
            // 
            // proxyActiveLabel
            // 
            this.proxyActiveLabel.AutoSize = true;
            this.proxyActiveLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.proxyActiveLabel.Location = new System.Drawing.Point(410, 267);
            this.proxyActiveLabel.Name = "proxyActiveLabel";
            this.proxyActiveLabel.Size = new System.Drawing.Size(136, 25);
            this.proxyActiveLabel.TabIndex = 17;
            this.proxyActiveLabel.Text = "Proxy is active!";
            this.proxyActiveLabel.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::WC3LanGame.Properties.Resources.WarcraftBackgroundImage;
            this.ClientSize = new System.Drawing.Size(624, 321);
            this.Controls.Add(this.proxyActiveLabel);
            this.Controls.Add(this.stopWC3Button);
            this.Controls.Add(this.scanningNetworkLabel);
            this.Controls.Add(this.hostAddressComboBox);
            this.Controls.Add(this.scanningNetworkProgressBar);
            this.Controls.Add(this.wc3ProcessRunningStatusLabel);
            this.Controls.Add(this.runWC3Button);
            this.Controls.Add(this.stopProxyButton);
            this.Controls.Add(this.gameInfoTableLayoutPanel);
            this.Controls.Add(this.runProxyButton);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.gameLabel);
            this.Controls.Add(this.hostLabel);
            this.Controls.Add(this.gameTypeComboBox);
            this.Controls.Add(this.wc3VersionComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "WC3 Lan Game";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.gameInfoTableLayoutPanel.ResumeLayout(false);
            this.gameInfoTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ComboBox wc3VersionComboBox;
        private ComboBox gameTypeComboBox;
        private Label hostLabel;
        private Label gameLabel;
        private Label versionLabel;
        private Button runProxyButton;
        private NotifyIcon notifyIcon;
        private TableLayoutPanel gameInfoTableLayoutPanel;
        private Label hostAddressTitleLabel;
        private Label hostAddressValueLabel;
        private Label gamePortTitleLabel;
        private Label gamePortValueLabel;
        private Label gameNameTitleLabel;
        private Label gameNameValueLabel;
        private Label mapNameTitleLabel;
        private Label mapNameValueLabel;
        private Label playersCountTitleLabel;
        private Label playersCountValueLabel;
        private Label clientCountTitleLabel;
        private Label clientCountValueLabel;
        private Button stopProxyButton;
        private Button runWC3Button;
        private Label wc3ProcessRunningStatusLabel;
        private ProgressBar scanningNetworkProgressBar;
        private ComboBox hostAddressComboBox;
        private Label scanningNetworkLabel;
        private Button stopWC3Button;
        private Label mapSizeValueLabel;
        private Label mapSizeTitleLabel;
        private Label gameTypeValueLabel;
        private Label gameTypeTitleLabel;
        private Label proxyActiveLabel;
    }
}