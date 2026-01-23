using SystemHealthDashboard.Core.Events;
using SystemHealthDashboard.Metrics;
using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.Core.Services;

public class ApplicationCore : IDisposable
{
    private readonly MetricManager _metricManager;
    private readonly MetricCache _metricCache;
    private readonly EventBus _eventBus;
    private readonly int _updateIntervalMs;
    private readonly int _historySize;

    public EventBus EventBus => _eventBus;
    public MetricCache Cache => _metricCache;
    public bool IsRunning => _metricManager.IsRunning;

    public ApplicationCore(int updateIntervalMs = 1000, int historySize = 60)
    {
        _updateIntervalMs = updateIntervalMs;
        _historySize = historySize;

        _metricManager = new MetricManager(updateIntervalMs, historySize);
        _metricCache = new MetricCache(historySize);
        _eventBus = new EventBus();

        _metricManager.CpuMetricUpdated += OnCpuMetricUpdated;
        _metricManager.MemoryMetricUpdated += OnMemoryMetricUpdated;
        _metricManager.DiskMetricUpdated += OnDiskMetricUpdated;
        _metricManager.NetworkMetricUpdated += OnNetworkMetricUpdated;
    }

    public void Start()
    {
        _metricManager.Start();
    }

    public void Stop()
    {
        _metricManager.Stop();
    }

    private void OnCpuMetricUpdated(object? sender, CpuMetricData metric)
    {
        _metricCache.UpdateCpu(metric);
        _eventBus.PublishCpuMetric(metric);
    }

    private void OnMemoryMetricUpdated(object? sender, MemoryMetricData metric)
    {
        _metricCache.UpdateMemory(metric);
        _eventBus.PublishMemoryMetric(metric);
    }

    private void OnDiskMetricUpdated(object? sender, DiskMetricData metric)
    {
        _metricCache.UpdateDisk(metric);
        _eventBus.PublishDiskMetric(metric);
    }

    private void OnNetworkMetricUpdated(object? sender, NetworkMetricData metric)
    {
        _metricCache.UpdateNetwork(metric);
        _eventBus.PublishNetworkMetric(metric);
    }

    public CpuMetricData? GetCurrentCpuMetric() => _metricCache.GetCurrentCpu();
    public MemoryMetricData? GetCurrentMemoryMetric() => _metricCache.GetCurrentMemory();
    public DiskMetricData? GetCurrentDiskMetric() => _metricCache.GetCurrentDisk();
    public NetworkMetricData? GetCurrentNetworkMetric() => _metricCache.GetCurrentNetwork();

    public IReadOnlyList<CpuMetricData> GetCpuHistory() => _metricCache.GetCpuHistory();
    public IReadOnlyList<MemoryMetricData> GetMemoryHistory() => _metricCache.GetMemoryHistory();
    public IReadOnlyList<DiskMetricData> GetDiskHistory() => _metricCache.GetDiskHistory();
    public IReadOnlyList<NetworkMetricData> GetNetworkHistory() => _metricCache.GetNetworkHistory();

    public void Dispose()
    {
        _metricManager?.Dispose();
    }
}
