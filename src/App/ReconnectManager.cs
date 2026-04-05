using Timer = System.Timers.Timer;

namespace WC3LanGame.App
{
    internal record ReconnectScheduledEventArgs(int Attempt, int DelaySeconds);

    internal sealed class ReconnectManager : IDisposable
    {
        private Timer _reconnectTimer;
        private int _reconnectAttempt;
        private bool _disposed;

        public bool IsReconnecting { get; private set; }

        public event EventHandler<ReconnectScheduledEventArgs> ReconnectScheduled;
        public event EventHandler ReconnectRequested;

        public void Start()
        {
            if (IsReconnecting) return;
            IsReconnecting = true;
            _reconnectAttempt = 0;
            ScheduleNext();
        }

        public void Stop()
        {
            IsReconnecting = false;
            _reconnectTimer?.Stop();
            _reconnectTimer?.Dispose();
            _reconnectTimer = null;
            _reconnectAttempt = 0;
        }

        public void OnReconnectSucceeded()
        {
            IsReconnecting = false;
            _reconnectAttempt = 0;
        }

        public void OnReconnectFailed()
        {
            ScheduleNext();
        }

        private void ScheduleNext()
        {
            _reconnectAttempt++;
            int delaySeconds = Math.Min(5 * _reconnectAttempt, 30);

            ReconnectScheduled?.Invoke(this, new ReconnectScheduledEventArgs(_reconnectAttempt, delaySeconds));

            _reconnectTimer?.Stop();
            _reconnectTimer?.Dispose();
            _reconnectTimer = new Timer(delaySeconds * 1000);
            _reconnectTimer.AutoReset = false;
            _reconnectTimer.Elapsed += (_, _) =>
            {
                if (_disposed || !IsReconnecting) return;
                ReconnectRequested?.Invoke(this, EventArgs.Empty);
            };
            _reconnectTimer.Start();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _reconnectTimer?.Stop();
            _reconnectTimer?.Dispose();
            _reconnectTimer = null;
        }
    }
}
