namespace SystemHealthDashboard.Core.Models;

public class AlertConfiguration
{
    public double CpuThresholdPercent { get; set; } = 85.0;
    public int CpuThresholdDurationSeconds { get; set; } = 10;
    
    public double MemoryThresholdPercent { get; set; } = 80.0;
    public int MemoryThresholdDurationSeconds { get; set; } = 10;
    
    public double DiskUsageThresholdPercent { get; set; } = 90.0;
    
    public bool NotificationsEnabled { get; set; } = true;
    public bool TrayIconColorChangeEnabled { get; set; } = true;
}

public enum AlertType
{
    CpuHigh,
    MemoryHigh,
    DiskAlmostFull
}

public enum AlertSeverity
{
    Normal,
    Warning,
    Critical
}

public class Alert
{
    public AlertType Type { get; set; }
    public AlertSeverity Severity { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
    public double Threshold { get; set; }

    public Alert(AlertType type, AlertSeverity severity, string message, double value, double threshold)
    {
        Type = type;
        Severity = severity;
        Message = message ?? string.Empty;
        Timestamp = DateTime.Now;
        Value = value;
        Threshold = threshold;
    }
}
