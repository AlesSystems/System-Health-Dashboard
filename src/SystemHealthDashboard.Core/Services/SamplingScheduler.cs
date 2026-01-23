namespace SystemHealthDashboard.Core.Services;

public class SamplingScheduler : IDisposable
{
    private readonly int _intervalMs;
    private Timer? _timer;
    private readonly Action _onTick;
    private bool _isRunning;

    public event EventHandler? Tick;

    public SamplingScheduler(int intervalMs, Action onTick)
    {
        if (intervalMs <= 0)
            throw new ArgumentException("Interval must be positive", nameof(intervalMs));

        _intervalMs = intervalMs;
        _onTick = onTick ?? throw new ArgumentNullException(nameof(onTick));
    }

    public int IntervalMs => _intervalMs;
    public bool IsRunning => _isRunning;

    public void Start()
    {
        if (_isRunning)
            return;

        _timer = new Timer(_ => 
        {
            _onTick();
            Tick?.Invoke(this, EventArgs.Empty);
        }, null, 0, _intervalMs);

        _isRunning = true;
    }

    public void Stop()
    {
        if (!_isRunning)
            return;

        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        _timer?.Dispose();
        _timer = null;
        _isRunning = false;
    }

    public void Dispose()
    {
        Stop();
    }
}
