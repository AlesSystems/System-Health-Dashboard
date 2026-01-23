using SystemHealthDashboard.Metrics.Models;
using Xunit;

namespace SystemHealthDashboard.Tests;

public class MetricParsingTests
{
    [Fact]
    public void CpuMetricData_CalculatesCorrectly()
    {
        var perCoreUsage = new List<double> { 25.5, 30.2, 45.1, 20.8 };
        var cpuMetric = new CpuMetricData(30.4, perCoreUsage);

        Assert.Equal(30.4, cpuMetric.TotalUsagePercent);
        Assert.Equal(30.4, cpuMetric.Value);
        Assert.Equal(4, cpuMetric.PerCoreUsage.Count);
        Assert.True(cpuMetric.Timestamp <= DateTime.Now);
    }

    [Fact]
    public void MemoryMetricData_CalculatesPercentageCorrectly()
    {
        long total = 16L * 1024 * 1024 * 1024; // 16 GB
        long used = 8L * 1024 * 1024 * 1024;   // 8 GB
        long available = total - used;

        var memoryMetric = new MemoryMetricData(total, used, available);

        Assert.Equal(total, memoryMetric.TotalBytes);
        Assert.Equal(used, memoryMetric.UsedBytes);
        Assert.Equal(available, memoryMetric.AvailableBytes);
        Assert.Equal(50.0, memoryMetric.UsagePercent, 1);
        Assert.Equal(50.0, memoryMetric.Value, 1);
    }

    [Fact]
    public void MemoryMetricData_HandlesZeroTotal()
    {
        var memoryMetric = new MemoryMetricData(0, 0, 0);

        Assert.Equal(0, memoryMetric.UsagePercent);
        Assert.Equal(0, memoryMetric.Value);
    }

    [Fact]
    public void DiskMetricData_CalculatesCorrectly()
    {
        long readBytes = 1024 * 1024;  // 1 MB/s
        long writeBytes = 512 * 1024;  // 512 KB/s

        var diskMetric = new DiskMetricData(readBytes, writeBytes);

        Assert.Equal(readBytes, diskMetric.ReadBytesPerSecond);
        Assert.Equal(writeBytes, diskMetric.WriteBytesPerSecond);
        Assert.Equal(readBytes + writeBytes, diskMetric.Value);
    }

    [Fact]
    public void NetworkMetricData_CalculatesCorrectly()
    {
        long downloadBytes = 2 * 1024 * 1024;  // 2 MB/s
        long uploadBytes = 1024 * 1024;        // 1 MB/s

        var networkMetric = new NetworkMetricData(downloadBytes, uploadBytes);

        Assert.Equal(downloadBytes, networkMetric.DownloadBytesPerSecond);
        Assert.Equal(uploadBytes, networkMetric.UploadBytesPerSecond);
        Assert.Equal(downloadBytes + uploadBytes, networkMetric.Value);
    }

    [Fact]
    public void MetricData_HasValidTimestamp()
    {
        var before = DateTime.Now;
        var metric = new MetricData(42.5);
        var after = DateTime.Now;

        Assert.True(metric.Timestamp >= before);
        Assert.True(metric.Timestamp <= after);
        Assert.Equal(42.5, metric.Value);
    }
}
