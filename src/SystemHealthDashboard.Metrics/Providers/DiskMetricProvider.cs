using System.Diagnostics;
using SystemHealthDashboard.Metrics.Interfaces;
using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.Metrics.Providers;

public class DiskMetricProvider : IMetricProvider<DiskMetricData>
{
    private PerformanceCounter _diskReadCounter;
    private PerformanceCounter _diskWriteCounter;

    public void Initialize()
    {
        _diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
        _diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");

        _diskReadCounter.NextValue();
        _diskWriteCounter.NextValue();
    }

    public DiskMetricData GetCurrentMetric()
    {
        if (_diskReadCounter == null || _diskWriteCounter == null)
        {
            Initialize();
        }

        long readBytes = (long)_diskReadCounter.NextValue();
        long writeBytes = (long)_diskWriteCounter.NextValue();

        return new DiskMetricData(readBytes, writeBytes);
    }

    public void Dispose()
    {
        _diskReadCounter?.Dispose();
        _diskWriteCounter?.Dispose();
    }
}
