using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.Metrics.Schedulers;

public class MetricScheduler<T> : IDisposable where T : MetricData
{
    private readonly Func<T> _metricCollector;
    private readonly int _intervalMs;
    private readonly int _historySize;
    private readonly RingBuffer<T> _historyBuffer;
    private Timer? _timer;
    private readonly object _lock = new object();
    private T? _currentMetric;

    public event EventHandler<T>? MetricUpdated;

    public MetricScheduler(Func<T> metricCollector, int intervalMs = 1000, int historySize = 60)
    {
        _metricCollector = metricCollector ?? throw new ArgumentNullException(nameof(metricCollector));
        _intervalMs = intervalMs;
        _historySize = historySize;
        _historyBuffer = new RingBuffer<T>(historySize);
    }

    public void Start()
    {
        if (_timer != null)
        {
            return;
        }

        _timer = new Timer(CollectMetric, null, 0, _intervalMs);
    }

    public void Stop()
    {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        _timer?.Dispose();
        _timer = null;
    }

    private void CollectMetric(object? state)
    {
        try
        {
            var metric = _metricCollector();
            
            lock (_lock)
            {
                _currentMetric = metric;
                _historyBuffer.Add(metric);
            }

            MetricUpdated?.Invoke(this, metric);
        }
        catch (Exception)
        {
            // Log error if logging is available
        }
    }

    public T? GetCurrentMetric()
    {
        lock (_lock)
        {
            return _currentMetric;
        }
    }

    public IReadOnlyList<T> GetHistory()
    {
        lock (_lock)
        {
            return _historyBuffer.GetAll();
        }
    }

    public void Dispose()
    {
        Stop();
    }
}
