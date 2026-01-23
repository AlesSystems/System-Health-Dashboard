using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.Core.Events;

public class EventBus
{
    public event EventHandler<CpuMetricEventArgs>? CpuMetricReceived;
    public event EventHandler<MemoryMetricEventArgs>? MemoryMetricReceived;
    public event EventHandler<DiskMetricEventArgs>? DiskMetricReceived;
    public event EventHandler<NetworkMetricEventArgs>? NetworkMetricReceived;

    public void PublishCpuMetric(CpuMetricData metric)
    {
        CpuMetricReceived?.Invoke(this, new CpuMetricEventArgs(metric));
    }

    public void PublishMemoryMetric(MemoryMetricData metric)
    {
        MemoryMetricReceived?.Invoke(this, new MemoryMetricEventArgs(metric));
    }

    public void PublishDiskMetric(DiskMetricData metric)
    {
        DiskMetricReceived?.Invoke(this, new DiskMetricEventArgs(metric));
    }

    public void PublishNetworkMetric(NetworkMetricData metric)
    {
        NetworkMetricReceived?.Invoke(this, new NetworkMetricEventArgs(metric));
    }
}
