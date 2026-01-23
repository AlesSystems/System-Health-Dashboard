using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.Core.Services;

public class MetricCache
{
    private readonly object _lock = new object();
    private CpuMetricData? _currentCpu;
    private MemoryMetricData? _currentMemory;
    private DiskMetricData? _currentDisk;
    private NetworkMetricData? _currentNetwork;

    private List<CpuMetricData> _cpuHistory = new();
    private List<MemoryMetricData> _memoryHistory = new();
    private List<DiskMetricData> _diskHistory = new();
    private List<NetworkMetricData> _networkHistory = new();

    private readonly int _maxHistorySize;

    public MetricCache(int maxHistorySize = 60)
    {
        _maxHistorySize = maxHistorySize;
    }

    public void UpdateCpu(CpuMetricData metric)
    {
        lock (_lock)
        {
            _currentCpu = metric;
            _cpuHistory.Add(metric);
            if (_cpuHistory.Count > _maxHistorySize)
            {
                _cpuHistory.RemoveAt(0);
            }
        }
    }

    public void UpdateMemory(MemoryMetricData metric)
    {
        lock (_lock)
        {
            _currentMemory = metric;
            _memoryHistory.Add(metric);
            if (_memoryHistory.Count > _maxHistorySize)
            {
                _memoryHistory.RemoveAt(0);
            }
        }
    }

    public void UpdateDisk(DiskMetricData metric)
    {
        lock (_lock)
        {
            _currentDisk = metric;
            _diskHistory.Add(metric);
            if (_diskHistory.Count > _maxHistorySize)
            {
                _diskHistory.RemoveAt(0);
            }
        }
    }

    public void UpdateNetwork(NetworkMetricData metric)
    {
        lock (_lock)
        {
            _currentNetwork = metric;
            _networkHistory.Add(metric);
            if (_networkHistory.Count > _maxHistorySize)
            {
                _networkHistory.RemoveAt(0);
            }
        }
    }

    public CpuMetricData? GetCurrentCpu()
    {
        lock (_lock)
        {
            return _currentCpu;
        }
    }

    public MemoryMetricData? GetCurrentMemory()
    {
        lock (_lock)
        {
            return _currentMemory;
        }
    }

    public DiskMetricData? GetCurrentDisk()
    {
        lock (_lock)
        {
            return _currentDisk;
        }
    }

    public NetworkMetricData? GetCurrentNetwork()
    {
        lock (_lock)
        {
            return _currentNetwork;
        }
    }

    public IReadOnlyList<CpuMetricData> GetCpuHistory()
    {
        lock (_lock)
        {
            return _cpuHistory.ToList();
        }
    }

    public IReadOnlyList<MemoryMetricData> GetMemoryHistory()
    {
        lock (_lock)
        {
            return _memoryHistory.ToList();
        }
    }

    public IReadOnlyList<DiskMetricData> GetDiskHistory()
    {
        lock (_lock)
        {
            return _diskHistory.ToList();
        }
    }

    public IReadOnlyList<NetworkMetricData> GetNetworkHistory()
    {
        lock (_lock)
        {
            return _networkHistory.ToList();
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _currentCpu = null;
            _currentMemory = null;
            _currentDisk = null;
            _currentNetwork = null;
            _cpuHistory.Clear();
            _memoryHistory.Clear();
            _diskHistory.Clear();
            _networkHistory.Clear();
        }
    }
}
