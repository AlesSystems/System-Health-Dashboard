using System.Diagnostics;
using SystemHealthDashboard.Core.Services;

namespace SystemHealthDashboard.Demo;

public class PerformanceBenchmark
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== System Health Dashboard - Phase 8 Performance Benchmark ===\n");

        var perfMonitor = new PerformanceMonitor();
        using var app = new ApplicationCore(updateIntervalMs: 1000, historySize: 60);

        var updateCount = 0;
        var startTime = DateTime.Now;

        // Subscribe to events to count updates
        app.EventBus.CpuMetricReceived += (s, e) => Interlocked.Increment(ref updateCount);
        app.EventBus.MemoryMetricReceived += (s, e) => Interlocked.Increment(ref updateCount);
        app.EventBus.DiskMetricReceived += (s, e) => Interlocked.Increment(ref updateCount);
        app.EventBus.NetworkMetricReceived += (s, e) => Interlocked.Increment(ref updateCount);

        Console.WriteLine("Starting monitoring...");
        Console.WriteLine("Collecting performance metrics for 30 seconds...\n");

        app.Start();

        var timer = new System.Timers.Timer(5000);
        var sampleCount = 0;
        var totalAppCpu = 0.0;
        var totalAppMemory = 0L;

        timer.Elapsed += (s, e) =>
        {
            sampleCount++;
            var metrics = perfMonitor.GetCurrentMetrics();
            totalAppCpu += metrics.CpuUsagePercent;
            totalAppMemory += metrics.MemoryUsageMB;
            
            Console.WriteLine($"[Sample {sampleCount}] {metrics}");
            Console.WriteLine($"  Total metric updates: {updateCount}");
            Console.WriteLine($"  Updates/second: {updateCount / ((DateTime.Now - startTime).TotalSeconds):F2}\n");
        };
        timer.Start();

        await Task.Delay(30000);

        timer.Stop();
        timer.Dispose();
        app.Stop();

        Console.WriteLine("\n=== Performance Summary ===\n");
        Console.WriteLine($"Test Duration: 30 seconds");
        Console.WriteLine($"Total Metric Updates: {updateCount}");
        Console.WriteLine($"Average Updates/Second: {updateCount / 30.0:F2}");
        Console.WriteLine($"\nApplication Resource Usage:");
        Console.WriteLine($"  Average CPU Usage: {totalAppCpu / sampleCount:F2}%");
        Console.WriteLine($"  Average Memory Usage: {totalAppMemory / sampleCount} MB");

        var finalMetrics = perfMonitor.GetCurrentMetrics();
        Console.WriteLine($"\nFinal Metrics: {finalMetrics}");

        Console.WriteLine("\n=== Performance Goals ===");
        Console.WriteLine("✓ Target: CPU < 5%");
        Console.WriteLine($"  Actual: {totalAppCpu / sampleCount:F2}%");

        var cpuGoalMet = (totalAppCpu / sampleCount) < 5.0;
        Console.ForegroundColor = cpuGoalMet ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"  Status: {(cpuGoalMet ? "PASS" : "FAIL")}");
        Console.ResetColor();

        Console.WriteLine("\n✓ Target: Memory < 100 MB");
        Console.WriteLine($"  Actual: {totalAppMemory / sampleCount} MB");

        var memoryGoalMet = (totalAppMemory / sampleCount) < 100;
        Console.ForegroundColor = memoryGoalMet ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"  Status: {(memoryGoalMet ? "PASS" : "FAIL")}");
        Console.ResetColor();

        Console.WriteLine("\n✓ Target: 4 updates/second (1 per metric type)");
        Console.WriteLine($"  Actual: {updateCount / 30.0:F2} updates/second");

        var updateGoalMet = Math.Abs((updateCount / 30.0) - 4.0) < 1.0;
        Console.ForegroundColor = updateGoalMet ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"  Status: {(updateGoalMet ? "PASS" : "FAIL")}");
        Console.ResetColor();

        Console.WriteLine("\n=== Benchmark Complete ===");
    }
}

