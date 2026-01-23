using System.Diagnostics;

namespace SystemHealthDashboard.Core.Services;

public class PerformanceMonitor
{
    private readonly Process _currentProcess;
    private DateTime _lastCheck;
    private TimeSpan _lastTotalProcessorTime;
    private readonly object _lock = new();

    public PerformanceMonitor()
    {
        _currentProcess = Process.GetCurrentProcess();
        _lastCheck = DateTime.UtcNow;
        _lastTotalProcessorTime = _currentProcess.TotalProcessorTime;
    }

    public double GetApplicationCpuUsage()
    {
        lock (_lock)
        {
            var currentTime = DateTime.UtcNow;
            var currentTotalProcessorTime = _currentProcess.TotalProcessorTime;

            var cpuUsedMs = (currentTotalProcessorTime - _lastTotalProcessorTime).TotalMilliseconds;
            var totalMsPassed = (currentTime - _lastCheck).TotalMilliseconds;

            var cpuUsageTotal = 0.0;
            if (totalMsPassed > 0)
            {
                cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            }

            _lastCheck = currentTime;
            _lastTotalProcessorTime = currentTotalProcessorTime;

            return cpuUsageTotal * 100;
        }
    }

    public long GetApplicationMemoryUsageMB()
    {
        _currentProcess.Refresh();
        return _currentProcess.WorkingSet64 / 1024 / 1024;
    }

    public PerformanceMetrics GetCurrentMetrics()
    {
        return new PerformanceMetrics
        {
            CpuUsagePercent = GetApplicationCpuUsage(),
            MemoryUsageMB = GetApplicationMemoryUsageMB(),
            ThreadCount = _currentProcess.Threads.Count,
            HandleCount = _currentProcess.HandleCount
        };
    }
}

public class PerformanceMetrics
{
    public double CpuUsagePercent { get; set; }
    public long MemoryUsageMB { get; set; }
    public int ThreadCount { get; set; }
    public int HandleCount { get; set; }

    public override string ToString()
    {
        return $"CPU: {CpuUsagePercent:F2}%, Memory: {MemoryUsageMB} MB, Threads: {ThreadCount}, Handles: {HandleCount}";
    }
}
