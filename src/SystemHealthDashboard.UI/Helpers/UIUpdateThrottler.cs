using System.Windows.Threading;

namespace SystemHealthDashboard.UI.Helpers;

public class UIUpdateThrottler : IDisposable
{
    private readonly DispatcherTimer _timer;
    private readonly Dictionary<string, Action> _pendingUpdates = new();
    private readonly object _lock = new();
    private bool _isDisposed;

    public UIUpdateThrottler(int intervalMs = 100)
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(intervalMs)
        };
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    public void ScheduleUpdate(string key, Action updateAction)
    {
        if (_isDisposed) return;

        lock (_lock)
        {
            _pendingUpdates[key] = updateAction;
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        Dictionary<string, Action> updates;
        
        lock (_lock)
        {
            if (_pendingUpdates.Count == 0)
                return;

            updates = new Dictionary<string, Action>(_pendingUpdates);
            _pendingUpdates.Clear();
        }

        foreach (var update in updates.Values)
        {
            try
            {
                update();
            }
            catch
            {
                // Ignore update errors
            }
        }
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        
        _isDisposed = true;
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
        
        lock (_lock)
        {
            _pendingUpdates.Clear();
        }
    }
}
