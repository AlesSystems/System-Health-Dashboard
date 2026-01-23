using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.UI.Models;

public class MetricUpdateBatch
{
    public CpuMetricData? CpuMetric { get; set; }
    public MemoryMetricData? MemoryMetric { get; set; }
    public DiskMetricData? DiskMetric { get; set; }
    public NetworkMetricData? NetworkMetric { get; set; }
    
    public bool HasCpuUpdate { get; set; }
    public bool HasMemoryUpdate { get; set; }
    public bool HasDiskUpdate { get; set; }
    public bool HasNetworkUpdate { get; set; }

    public void Clear()
    {
        CpuMetric = null;
        MemoryMetric = null;
        DiskMetric = null;
        NetworkMetric = null;
        HasCpuUpdate = false;
        HasMemoryUpdate = false;
        HasDiskUpdate = false;
        HasNetworkUpdate = false;
    }

    public bool HasAnyUpdate => HasCpuUpdate || HasMemoryUpdate || HasDiskUpdate || HasNetworkUpdate;
}
