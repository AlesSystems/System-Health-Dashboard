using SystemHealthDashboard.Core.Services;
using SystemHealthDashboard.Core.Models;
using SystemHealthDashboard.Demo;

Console.WriteLine("=== System Health Dashboard - Demo ===\n");
Console.WriteLine("Select mode:");
Console.WriteLine("1. Phase 7 - Alerts Demo");
Console.WriteLine("2. Phase 8 - Performance Benchmark");
Console.Write("\nEnter choice (1 or 2): ");

var choice = Console.ReadLine();

if (choice == "2")
{
    await PerformanceBenchmark.RunAsync();
    return;
}

Console.WriteLine("\n=== Phase 7 - Alerts & Notifications Demo ===");
Console.WriteLine("Application Core with Alerts Test\n");

using var app = new ApplicationCore(updateIntervalMs: 1000, historySize: 60);

// Configure alert thresholds (lower for demo purposes)
app.Alerts.Configuration.CpuThresholdPercent = 50.0;
app.Alerts.Configuration.CpuThresholdDurationSeconds = 5;
app.Alerts.Configuration.MemoryThresholdPercent = 60.0;
app.Alerts.Configuration.MemoryThresholdDurationSeconds = 5;
app.Alerts.Configuration.DiskUsageThresholdPercent = 90.0;

Console.WriteLine($"Alert Configuration:");
Console.WriteLine($"  CPU Threshold: {app.Alerts.Configuration.CpuThresholdPercent}% for {app.Alerts.Configuration.CpuThresholdDurationSeconds}s");
Console.WriteLine($"  Memory Threshold: {app.Alerts.Configuration.MemoryThresholdPercent}% for {app.Alerts.Configuration.MemoryThresholdDurationSeconds}s");
Console.WriteLine($"  Disk Threshold: {app.Alerts.Configuration.DiskUsageThresholdPercent}%\n");

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

app.Alerts.AlertTriggered += (s, alert) =>
{
    Console.ForegroundColor = alert.Severity == AlertSeverity.Critical ? ConsoleColor.Red : ConsoleColor.Yellow;
    Console.WriteLine($"\n*** ALERT [{alert.Severity}] *** {alert.Message}");
    Console.WriteLine($"    Value: {alert.Value:F2}, Threshold: {alert.Threshold:F2}");
    Console.ResetColor();
};

app.Alerts.SeverityChanged += (s, severity) =>
{
    var color = severity switch
    {
        AlertSeverity.Critical => ConsoleColor.Red,
        AlertSeverity.Warning => ConsoleColor.Yellow,
        _ => ConsoleColor.Green
    };
    Console.ForegroundColor = color;
    Console.WriteLine($"\n>>> System Severity Changed: {severity} <<<\n");
    Console.ResetColor();
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

Console.WriteLine($"\nFinal Alert Severity: {app.Alerts.CurrentSeverity}");
Console.WriteLine("\n=== Demo Complete ===");

