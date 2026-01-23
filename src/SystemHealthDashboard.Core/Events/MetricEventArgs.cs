using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.Core.Events;

public class MetricEventArgs : EventArgs
{
    public MetricData Metric { get; }
    public DateTime Timestamp { get; }

    public MetricEventArgs(MetricData metric)
    {
        Metric = metric ?? throw new ArgumentNullException(nameof(metric));
        Timestamp = metric.Timestamp;
    }
}

public class CpuMetricEventArgs : MetricEventArgs
{
    public new CpuMetricData Metric => (CpuMetricData)base.Metric;

    public CpuMetricEventArgs(CpuMetricData metric) : base(metric)
    {
    }
}

public class MemoryMetricEventArgs : MetricEventArgs
{
    public new MemoryMetricData Metric => (MemoryMetricData)base.Metric;

    public MemoryMetricEventArgs(MemoryMetricData metric) : base(metric)
    {
    }
}

public class DiskMetricEventArgs : MetricEventArgs
{
    public new DiskMetricData Metric => (DiskMetricData)base.Metric;

    public DiskMetricEventArgs(DiskMetricData metric) : base(metric)
    {
    }
}

public class NetworkMetricEventArgs : MetricEventArgs
{
    public new NetworkMetricData Metric => (NetworkMetricData)base.Metric;

    public NetworkMetricEventArgs(NetworkMetricData metric) : base(metric)
    {
    }
}
