namespace SystemHealthDashboard.Metrics.Models;

public class MetricData
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }

    public MetricData(double value)
    {
        Timestamp = DateTime.Now;
        Value = value;
    }
}

public class CpuMetricData : MetricData
{
    public double TotalUsagePercent { get; set; }
    public List<double> PerCoreUsage { get; set; }

    public CpuMetricData(double totalUsage, List<double> perCoreUsage) : base(totalUsage)
    {
        TotalUsagePercent = totalUsage;
        PerCoreUsage = perCoreUsage ?? new List<double>();
    }
}

public class MemoryMetricData : MetricData
{
    public long TotalBytes { get; set; }
    public long UsedBytes { get; set; }
    public long AvailableBytes { get; set; }
    public double UsagePercent { get; set; }

    public MemoryMetricData(long total, long used, long available) : base(0)
    {
        TotalBytes = total;
        UsedBytes = used;
        AvailableBytes = available;
        UsagePercent = total > 0 ? (used / (double)total) * 100 : 0;
        Value = UsagePercent;
    }
}

public class DiskMetricData : MetricData
{
    public long ReadBytesPerSecond { get; set; }
    public long WriteBytesPerSecond { get; set; }

    public DiskMetricData(long readBytes, long writeBytes) : base(readBytes + writeBytes)
    {
        ReadBytesPerSecond = readBytes;
        WriteBytesPerSecond = writeBytes;
    }
}

public class NetworkMetricData : MetricData
{
    public long DownloadBytesPerSecond { get; set; }
    public long UploadBytesPerSecond { get; set; }

    public NetworkMetricData(long downloadBytes, long uploadBytes) : base(downloadBytes + uploadBytes)
    {
        DownloadBytesPerSecond = downloadBytes;
        UploadBytesPerSecond = uploadBytes;
    }
}
