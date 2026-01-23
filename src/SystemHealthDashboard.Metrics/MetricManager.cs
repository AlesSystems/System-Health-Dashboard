using SystemHealthDashboard.Metrics.Interfaces;
using SystemHealthDashboard.Metrics.Models;
using SystemHealthDashboard.Metrics.Providers;
using SystemHealthDashboard.Metrics.Schedulers;

namespace SystemHealthDashboard.Metrics;

public class MetricManager : IDisposable
{
    private readonly CpuMetricProvider _cpuProvider;
    private readonly MemoryMetricProvider _memoryProvider;
    private readonly DiskMetricProvider _diskProvider;
    private readonly NetworkMetricProvider _networkProvider;

    private readonly MetricScheduler<CpuMetricData> _cpuScheduler;
    private readonly MetricScheduler<MemoryMetricData> _memoryScheduler;
    private readonly MetricScheduler<DiskMetricData> _diskScheduler;
    private readonly MetricScheduler<NetworkMetricData> _networkScheduler;

    private bool _isRunning;

    public event EventHandler<CpuMetricData>? CpuMetricUpdated;
    public event EventHandler<MemoryMetricData>? MemoryMetricUpdated;
    public event EventHandler<DiskMetricData>? DiskMetricUpdated;
    public event EventHandler<NetworkMetricData>? NetworkMetricUpdated;

    public MetricManager(int updateIntervalMs = 1000, int historySize = 60)
    {
        _cpuProvider = new CpuMetricProvider();
        _memoryProvider = new MemoryMetricProvider();
        _diskProvider = new DiskMetricProvider();
        _networkProvider = new NetworkMetricProvider();

        _cpuScheduler = new MetricScheduler<CpuMetricData>(
            () => _cpuProvider.GetCurrentMetric(),
            updateIntervalMs,
            historySize);

        _memoryScheduler = new MetricScheduler<MemoryMetricData>(
            () => _memoryProvider.GetCurrentMetric(),
            updateIntervalMs,
            historySize);

        _diskScheduler = new MetricScheduler<DiskMetricData>(
            () => _diskProvider.GetCurrentMetric(),
            updateIntervalMs,
            historySize);

        _networkScheduler = new MetricScheduler<NetworkMetricData>(
            () => _networkProvider.GetCurrentMetric(),
            updateIntervalMs,
            historySize);

        _cpuScheduler.MetricUpdated += (s, e) => CpuMetricUpdated?.Invoke(this, e);
        _memoryScheduler.MetricUpdated += (s, e) => MemoryMetricUpdated?.Invoke(this, e);
        _diskScheduler.MetricUpdated += (s, e) => DiskMetricUpdated?.Invoke(this, e);
        _networkScheduler.MetricUpdated += (s, e) => NetworkMetricUpdated?.Invoke(this, e);
    }

    public void Initialize()
    {
        _cpuProvider.Initialize();
        _memoryProvider.Initialize();
        _diskProvider.Initialize();
        _networkProvider.Initialize();
    }

    public void Start()
    {
        if (_isRunning)
        {
            return;
        }

        Initialize();

        _cpuScheduler.Start();
        _memoryScheduler.Start();
        _diskScheduler.Start();
        _networkScheduler.Start();

        _isRunning = true;
    }

    public void Stop()
    {
        if (!_isRunning)
        {
            return;
        }

        _cpuScheduler.Stop();
        _memoryScheduler.Stop();
        _diskScheduler.Stop();
        _networkScheduler.Stop();

        _isRunning = false;
    }

    public CpuMetricData? GetCurrentCpuMetric() => _cpuScheduler.GetCurrentMetric();
    public MemoryMetricData? GetCurrentMemoryMetric() => _memoryScheduler.GetCurrentMetric();
    public DiskMetricData? GetCurrentDiskMetric() => _diskScheduler.GetCurrentMetric();
    public NetworkMetricData? GetCurrentNetworkMetric() => _networkScheduler.GetCurrentMetric();

    public IReadOnlyList<CpuMetricData> GetCpuHistory() => _cpuScheduler.GetHistory();
    public IReadOnlyList<MemoryMetricData> GetMemoryHistory() => _memoryScheduler.GetHistory();
    public IReadOnlyList<DiskMetricData> GetDiskHistory() => _diskScheduler.GetHistory();
    public IReadOnlyList<NetworkMetricData> GetNetworkHistory() => _networkScheduler.GetHistory();

    public bool IsRunning => _isRunning;

    public void Dispose()
    {
        Stop();

        _cpuScheduler?.Dispose();
        _memoryScheduler?.Dispose();
        _diskScheduler?.Dispose();
        _networkScheduler?.Dispose();

        _cpuProvider?.Dispose();
        _memoryProvider?.Dispose();
        _diskProvider?.Dispose();
        _networkProvider?.Dispose();
    }
}
