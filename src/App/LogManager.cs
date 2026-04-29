namespace WC3LanGame.App;

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

        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var line = $"[{timestamp}] {message}{Environment.NewLine}";

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

        var palette = ThemePalette.Current;

        if (_richTextBox.Lines.Length > MaxLogLines)
        {
            _richTextBox.SelectAll();
            _richTextBox.SelectedText = "";
        }

        var color = level switch
        {
            LogLevel.Success => palette.LogSuccess,
            LogLevel.Warning => palette.LogWarning,
            LogLevel.Error => palette.LogError,
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
