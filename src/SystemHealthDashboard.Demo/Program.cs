using SystemHealthDashboard.Metrics;

Console.WriteLine("=== System Health Dashboard - Phase 4 Demo ===");
Console.WriteLine("Metric Collection Layer Test\n");

using var metricManager = new MetricManager(updateIntervalMs: 1000, historySize: 60);

metricManager.CpuMetricUpdated += (s, e) =>
{
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] CPU: {e.TotalUsagePercent:F2}% (Cores: {string.Join(", ", e.PerCoreUsage.Select(c => $"{c:F1}%"))})");
};

metricManager.MemoryMetricUpdated += (s, e) =>
{
    var usedMB = e.UsedBytes / 1024.0 / 1024.0;
    var totalMB = e.TotalBytes / 1024.0 / 1024.0;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Memory: {usedMB:F2} MB / {totalMB:F2} MB ({e.UsagePercent:F2}%)");
};

metricManager.DiskMetricUpdated += (s, e) =>
{
    var readKB = e.ReadBytesPerSecond / 1024.0;
    var writeKB = e.WriteBytesPerSecond / 1024.0;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Disk: Read {readKB:F2} KB/s, Write {writeKB:F2} KB/s");
};

metricManager.NetworkMetricUpdated += (s, e) =>
{
    var downloadKB = e.DownloadBytesPerSecond / 1024.0;
    var uploadKB = e.UploadBytesPerSecond / 1024.0;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Network: Down {downloadKB:F2} KB/s, Up {uploadKB:F2} KB/s");
};

Console.WriteLine("Starting metric collection...\n");
metricManager.Start();

Console.WriteLine("Press any key to stop and view history...\n");
Console.ReadKey();

metricManager.Stop();

Console.WriteLine("\n=== Metric History (Last 10 samples) ===\n");

var cpuHistory = metricManager.GetCpuHistory().TakeLast(10);
Console.WriteLine("CPU History:");
foreach (var metric in cpuHistory)
{
    Console.WriteLine($"  [{metric.Timestamp:HH:mm:ss}] {metric.TotalUsagePercent:F2}%");
}

var memoryHistory = metricManager.GetMemoryHistory().TakeLast(10);
Console.WriteLine("\nMemory History:");
foreach (var metric in memoryHistory)
{
    var usedMB = metric.UsedBytes / 1024.0 / 1024.0;
    Console.WriteLine($"  [{metric.Timestamp:HH:mm:ss}] {usedMB:F2} MB ({metric.UsagePercent:F2}%)");
}

Console.WriteLine("\n=== Demo Complete ===");
