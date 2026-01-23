using System.Collections.Concurrent;
using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.Core.Services;

public class OptimizedMetricCache
{
    private CpuMetricData? _currentCpu;
    private MemoryMetricData? _currentMemory;
    private DiskMetricData? _currentDisk;
    private NetworkMetricData? _currentNetwork;

    private readonly ConcurrentQueue<CpuMetricData> _cpuHistory = new();
    private readonly ConcurrentQueue<MemoryMetricData> _memoryHistory = new();
    private readonly ConcurrentQueue<DiskMetricData> _diskHistory = new();
    private readonly ConcurrentQueue<NetworkMetricData> _networkHistory = new();

    private readonly int _maxHistorySize;
    private int _cpuHistoryCount;
    private int _memoryHistoryCount;
    private int _diskHistoryCount;
    private int _networkHistoryCount;

    public OptimizedMetricCache(int maxHistorySize = 60)
    {
        _maxHistorySize = maxHistorySize;
    }

    public void UpdateCpu(CpuMetricData metric)
    {
        _currentCpu = metric;
        _cpuHistory.Enqueue(metric);
        
        if (Interlocked.Increment(ref _cpuHistoryCount) > _maxHistorySize)
        {
            _cpuHistory.TryDequeue(out _);
            Interlocked.Decrement(ref _cpuHistoryCount);
        }
    }

    public void UpdateMemory(MemoryMetricData metric)
    {
        _currentMemory = metric;
        _memoryHistory.Enqueue(metric);
        
        if (Interlocked.Increment(ref _memoryHistoryCount) > _maxHistorySize)
        {
            _memoryHistory.TryDequeue(out _);
            Interlocked.Decrement(ref _memoryHistoryCount);
        }
    }

    public void UpdateDisk(DiskMetricData metric)
    {
        _currentDisk = metric;
        _diskHistory.Enqueue(metric);
        
        if (Interlocked.Increment(ref _diskHistoryCount) > _maxHistorySize)
        {
            _diskHistory.TryDequeue(out _);
            Interlocked.Decrement(ref _diskHistoryCount);
        }
    }

    public void UpdateNetwork(NetworkMetricData metric)
    {
        _currentNetwork = metric;
        _networkHistory.Enqueue(metric);
        
        if (Interlocked.Increment(ref _networkHistoryCount) > _maxHistorySize)
        {
            _networkHistory.TryDequeue(out _);
            Interlocked.Decrement(ref _networkHistoryCount);
        }
    }

    public CpuMetricData? GetCurrentCpu() => _currentCpu;
    public MemoryMetricData? GetCurrentMemory() => _currentMemory;
    public DiskMetricData? GetCurrentDisk() => _currentDisk;
    public NetworkMetricData? GetCurrentNetwork() => _currentNetwork;

    public IReadOnlyList<CpuMetricData> GetCpuHistory() => _cpuHistory.ToArray();
    public IReadOnlyList<MemoryMetricData> GetMemoryHistory() => _memoryHistory.ToArray();
    public IReadOnlyList<DiskMetricData> GetDiskHistory() => _diskHistory.ToArray();
    public IReadOnlyList<NetworkMetricData> GetNetworkHistory() => _networkHistory.ToArray();

    public void Clear()
    {
        _currentCpu = null;
        _currentMemory = null;
        _currentDisk = null;
        _currentNetwork = null;
        
        while (_cpuHistory.TryDequeue(out _)) { }
        while (_memoryHistory.TryDequeue(out _)) { }
        while (_diskHistory.TryDequeue(out _)) { }
        while (_networkHistory.TryDequeue(out _)) { }
        
        _cpuHistoryCount = 0;
        _memoryHistoryCount = 0;
        _diskHistoryCount = 0;
        _networkHistoryCount = 0;
    }
}
