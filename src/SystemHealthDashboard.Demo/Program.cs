using SystemHealthDashboard.Core.Services;

Console.WriteLine("=== System Health Dashboard - Phase 5 Demo ===");
Console.WriteLine("Application Core Test\n");

using var app = new ApplicationCore(updateIntervalMs: 1000, historySize: 60);

app.EventBus.CpuMetricReceived += (s, e) =>
{
    var metric = e.Metric;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] CPU: {metric.TotalUsagePercent:F2}% (Cores: {string.Join(", ", metric.PerCoreUsage.Select(c => $"{c:F1}%"))})");
};

app.EventBus.MemoryMetricReceived += (s, e) =>
{
    var metric = e.Metric;
    var usedMB = metric.UsedBytes / 1024.0 / 1024.0;
    var totalMB = metric.TotalBytes / 1024.0 / 1024.0;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Memory: {usedMB:F2} MB / {totalMB:F2} MB ({metric.UsagePercent:F2}%)");
};

app.EventBus.DiskMetricReceived += (s, e) =>
{
    var metric = e.Metric;
    var readKB = metric.ReadBytesPerSecond / 1024.0;
    var writeKB = metric.WriteBytesPerSecond / 1024.0;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Disk: Read {readKB:F2} KB/s, Write {writeKB:F2} KB/s");
};

app.EventBus.NetworkMetricReceived += (s, e) =>
{
    var metric = e.Metric;
    var downloadKB = metric.DownloadBytesPerSecond / 1024.0;
    var uploadKB = metric.UploadBytesPerSecond / 1024.0;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Network: Down {downloadKB:F2} KB/s, Up {uploadKB:F2} KB/s");
};

Console.WriteLine("Starting Application Core...\n");
app.Start();

Console.WriteLine("Press any key to stop and view history...\n");
Console.ReadKey();

app.Stop();

Console.WriteLine("\n=== Metric History (Last 10 samples) ===\n");

var cpuHistory = app.GetCpuHistory().TakeLast(10);
Console.WriteLine("CPU History:");
foreach (var metric in cpuHistory)
{
    Console.WriteLine($"  [{metric.Timestamp:HH:mm:ss}] {metric.TotalUsagePercent:F2}%");
}

var memoryHistory = app.GetMemoryHistory().TakeLast(10);
Console.WriteLine("\nMemory History:");
foreach (var metric in memoryHistory)
{
    var usedMB = metric.UsedBytes / 1024.0 / 1024.0;
    Console.WriteLine($"  [{metric.Timestamp:HH:mm:ss}] {usedMB:F2} MB ({metric.UsagePercent:F2}%)");
}

Console.WriteLine("\n=== Demo Complete ===");
