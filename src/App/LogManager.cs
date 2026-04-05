namespace WC3LanGame.App
{
    internal enum LogLevel { Info, Success, Warning, Error }

    internal sealed class LogManager
    {
        private const int MaxLogLines = 500;

        private readonly RichTextBox _richTextBox;
        private readonly ToolStripStatusLabel _statusLabel;

        public LogManager(RichTextBox richTextBox, ToolStripStatusLabel statusLabel)
        {
            _richTextBox = richTextBox;
            _statusLabel = statusLabel;
        }

        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            if (_richTextBox.IsDisposed)
                return;

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string line = $"[{timestamp}] {message}{Environment.NewLine}";

            if (_richTextBox.InvokeRequired)
            {
                _richTextBox.BeginInvoke(() => AppendLog(line, message, level));
                return;
            }

            AppendLog(line, message, level);
        }

        private void AppendLog(string line, string statusText, LogLevel level)
        {
            if (_richTextBox.IsDisposed)
                return;

            if (_richTextBox.Lines.Length > MaxLogLines)
            {
                _richTextBox.SelectAll();
                _richTextBox.SelectedText = "";
            }

            Color color = level switch
            {
                LogLevel.Success => Color.FromArgb(0, 160, 0),
                LogLevel.Warning => Color.FromArgb(200, 160, 0),
                LogLevel.Error => Color.FromArgb(220, 50, 50),
                _ => _richTextBox.ForeColor
            };

            _richTextBox.SelectionStart = _richTextBox.TextLength;
            _richTextBox.SelectionLength = 0;
            _richTextBox.SelectionColor = color;
            _richTextBox.AppendText(line);
            _richTextBox.ScrollToCaret();

            _statusLabel.Text = statusText;
        }
    }
}
