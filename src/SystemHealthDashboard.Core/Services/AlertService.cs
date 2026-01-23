using SystemHealthDashboard.Core.Models;
using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.Core.Services;

public class AlertService
{
    private readonly AlertConfiguration _config;
    private readonly Queue<DateTime> _cpuHighTimestamps;
    private readonly Queue<DateTime> _memoryHighTimestamps;
    private DateTime? _lastCpuAlertTime;
    private DateTime? _lastMemoryAlertTime;
    private DateTime? _lastDiskAlertTime;
    private readonly TimeSpan _alertCooldown = TimeSpan.FromMinutes(5);

    public event EventHandler<Alert>? AlertTriggered;
    public event EventHandler<AlertSeverity>? SeverityChanged;

    private AlertSeverity _currentSeverity = AlertSeverity.Normal;

    public AlertService(AlertConfiguration? config = null)
    {
        _config = config ?? new AlertConfiguration();
        _cpuHighTimestamps = new Queue<DateTime>();
        _memoryHighTimestamps = new Queue<DateTime>();
    }

    public AlertConfiguration Configuration => _config;

    public AlertSeverity CurrentSeverity
    {
        get => _currentSeverity;
        private set
        {
            if (_currentSeverity != value)
            {
                _currentSeverity = value;
                SeverityChanged?.Invoke(this, value);
            }
        }
    }

    public void CheckCpuMetric(CpuMetricData metric)
    {
        var now = DateTime.Now;
        
        CleanOldTimestamps(_cpuHighTimestamps, now, _config.CpuThresholdDurationSeconds);

        if (metric.TotalUsagePercent >= _config.CpuThresholdPercent)
        {
            _cpuHighTimestamps.Enqueue(now);

            if (_cpuHighTimestamps.Count >= _config.CpuThresholdDurationSeconds &&
                (!_lastCpuAlertTime.HasValue || now - _lastCpuAlertTime.Value > _alertCooldown))
            {
                var alert = new Alert(
                    AlertType.CpuHigh,
                    AlertSeverity.Warning,
                    $"CPU usage is high: {metric.TotalUsagePercent:F1}%",
                    metric.TotalUsagePercent,
                    _config.CpuThresholdPercent
                );

                _lastCpuAlertTime = now;
                AlertTriggered?.Invoke(this, alert);
                UpdateSeverity();
            }
        }
        else
        {
            _cpuHighTimestamps.Clear();
            UpdateSeverity();
        }
    }

    public void CheckMemoryMetric(MemoryMetricData metric)
    {
        var now = DateTime.Now;
        
        CleanOldTimestamps(_memoryHighTimestamps, now, _config.MemoryThresholdDurationSeconds);

        if (metric.UsagePercent >= _config.MemoryThresholdPercent)
        {
            _memoryHighTimestamps.Enqueue(now);

            if (_memoryHighTimestamps.Count >= _config.MemoryThresholdDurationSeconds &&
                (!_lastMemoryAlertTime.HasValue || now - _lastMemoryAlertTime.Value > _alertCooldown))
            {
                var alert = new Alert(
                    AlertType.MemoryHigh,
                    AlertSeverity.Warning,
                    $"Memory usage is high: {metric.UsagePercent:F1}%",
                    metric.UsagePercent,
                    _config.MemoryThresholdPercent
                );

                _lastMemoryAlertTime = now;
                AlertTriggered?.Invoke(this, alert);
                UpdateSeverity();
            }
        }
        else
        {
            _memoryHighTimestamps.Clear();
            UpdateSeverity();
        }
    }

    public void CheckDiskMetric(DiskMetricData metric)
    {
        var now = DateTime.Now;

        try
        {
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives.Where(d => d.IsReady && d.DriveType == DriveType.Fixed))
            {
                var usedPercent = (1.0 - ((double)drive.AvailableFreeSpace / drive.TotalSize)) * 100;

                if (usedPercent >= _config.DiskUsageThresholdPercent &&
                    (!_lastDiskAlertTime.HasValue || now - _lastDiskAlertTime.Value > _alertCooldown))
                {
                    var alert = new Alert(
                        AlertType.DiskAlmostFull,
                        AlertSeverity.Critical,
                        $"Disk {drive.Name} is almost full: {usedPercent:F1}%",
                        usedPercent,
                        _config.DiskUsageThresholdPercent
                    );

                    _lastDiskAlertTime = now;
                    AlertTriggered?.Invoke(this, alert);
                    UpdateSeverity();
                    break;
                }
            }
        }
        catch
        {
            // Ignore disk check errors
        }
    }

    private void CleanOldTimestamps(Queue<DateTime> queue, DateTime now, int thresholdSeconds)
    {
        var cutoff = now.AddSeconds(-thresholdSeconds);
        while (queue.Count > 0 && queue.Peek() < cutoff)
        {
            queue.Dequeue();
        }
    }

    private void UpdateSeverity()
    {
        if (_cpuHighTimestamps.Count >= _config.CpuThresholdDurationSeconds ||
            _memoryHighTimestamps.Count >= _config.MemoryThresholdDurationSeconds)
        {
            CurrentSeverity = AlertSeverity.Warning;
        }
        else
        {
            CurrentSeverity = AlertSeverity.Normal;
        }
    }
}
