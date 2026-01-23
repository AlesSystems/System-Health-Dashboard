namespace SystemHealthDashboard.Core.Models;

public class ApplicationSettings
{
    public int RefreshIntervalMs { get; set; } = 1000;
    public int HistorySize { get; set; } = 60;
    
    public ThresholdSettings Thresholds { get; set; } = new();
    public ThemeSettings Theme { get; set; } = new();
    public StartupSettings Startup { get; set; } = new();
}

public class ThresholdSettings
{
    public double CpuThresholdPercent { get; set; } = 85.0;
    public int CpuThresholdDurationSeconds { get; set; } = 10;
    
    public double MemoryThresholdPercent { get; set; } = 80.0;
    public int MemoryThresholdDurationSeconds { get; set; } = 10;
    
    public double DiskUsageThresholdPercent { get; set; } = 90.0;
    
    public bool NotificationsEnabled { get; set; } = true;
    public bool TrayIconColorChangeEnabled { get; set; } = true;
}

public class ThemeSettings
{
    public string Theme { get; set; } = "Dark";
    public string AccentColor { get; set; } = "#FF6495ED";
}

public class StartupSettings
{
    public bool StartMinimized { get; set; } = false;
    public bool StartWithWindows { get; set; } = false;
    public bool MinimizeToTray { get; set; } = true;
}
