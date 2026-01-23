# Phase 7 — Alerts & Notifications

Phase 7 implements a comprehensive alert and notification system for monitoring system health metrics.

## Implemented Features

### 1. Alert System (`AlertService`)

- **Configurable Thresholds**: Set custom thresholds for CPU, Memory, and Disk usage
- **Duration-based Alerts**: CPU and Memory alerts only trigger after sustained high usage
- **Alert Cooldown**: 5-minute cooldown period between repeated alerts
- **Three Alert Types**:
  - CPU High: When CPU usage exceeds threshold for X seconds
  - Memory High: When memory usage exceeds threshold for X seconds
  - Disk Almost Full: When disk usage exceeds threshold

### 2. Alert Configuration

Default thresholds can be customized:
```csharp
var config = new AlertConfiguration
{
    CpuThresholdPercent = 85.0,           // Alert when CPU > 85%
    CpuThresholdDurationSeconds = 10,      // For 10 seconds
    MemoryThresholdPercent = 80.0,         // Alert when Memory > 80%
    MemoryThresholdDurationSeconds = 10,   // For 10 seconds
    DiskUsageThresholdPercent = 90.0,      // Alert when Disk > 90%
    NotificationsEnabled = true,
    TrayIconColorChangeEnabled = true
};
```

### 3. Alert Severity Levels

- **Normal**: All metrics within acceptable ranges (Green tray icon)
- **Warning**: CPU or Memory exceeds threshold (Orange/Yellow tray icon)
- **Critical**: Disk almost full (Red tray icon)

### 4. Desktop Notifications

- MessageBox alerts when thresholds are exceeded
- Tray icon balloon notifications (when using TaskbarIcon)
- Can be disabled via configuration

### 5. System Tray Icon

- **Minimize to Tray**: Window minimizes to system tray
- **Color-coded Icon**: Changes color based on alert severity
  - Green: Normal operation
  - Orange/Yellow: Warning level
  - Red: Critical alert
- **Tray Menu**:
  - Show Dashboard (bold)
  - Exit
- **Click to Restore**: Left-click tray icon to restore window

### 6. Integration with ApplicationCore

The AlertService is fully integrated into the ApplicationCore:

```csharp
using var app = new ApplicationCore();

// Configure alerts
app.Alerts.Configuration.CpuThresholdPercent = 75.0;

// Subscribe to alert events
app.Alerts.AlertTriggered += (s, alert) => 
{
    Console.WriteLine($"Alert: {alert.Message}");
};

app.Alerts.SeverityChanged += (s, severity) =>
{
    Console.WriteLine($"System severity: {severity}");
};

app.Start();
```

## Architecture

### Core Components

1. **AlertConfiguration** (`Core/Models/AlertConfiguration.cs`)
   - Configuration model for alert thresholds
   - Alert types and severity enums

2. **AlertService** (`Core/Services/AlertService.cs`)
   - Monitors metrics against thresholds
   - Tracks duration of high usage
   - Manages alert cooldowns
   - Raises events for alerts and severity changes

3. **NotificationService** (`UI/Services/NotificationService.cs`)
   - Displays desktop notifications
   - Shows MessageBox alerts
   - Integrates with tray icon

4. **TrayIconHelper** (`UI/Helpers/TrayIconHelper.cs`)
   - Generates dynamic tray icons
   - Color-codes based on severity

### Event Flow

```
MetricProvider → ApplicationCore → AlertService
                                          ↓
                                   AlertTriggered Event
                                          ↓
                                  NotificationService
                                          ↓
                                  Desktop Notification
```

## Usage Examples

### Console Application

```csharp
using var app = new ApplicationCore();

// Lower thresholds for testing
app.Alerts.Configuration.CpuThresholdPercent = 50.0;
app.Alerts.Configuration.CpuThresholdDurationSeconds = 5;

app.Alerts.AlertTriggered += (s, alert) =>
{
    Console.WriteLine($"⚠️ {alert.Message}");
    Console.WriteLine($"   Value: {alert.Value:F2}%");
};

app.Start();
```

### WPF Application

The UI automatically:
- Shows MessageBox alerts when thresholds are exceeded
- Updates tray icon color based on severity
- Minimizes to tray when window is minimized
- Restores from tray on left-click

## Testing

Run the demo application to see alerts in action:

```bash
cd src/SystemHealthDashboard.Demo
dotnet run --configuration Release
```

The demo sets lower thresholds (CPU: 50%, Memory: 60%) to make it easier to trigger alerts during testing.

## Dependencies

- **Hardcodet.NotifyIcon.Wpf** (1.1.0): System tray icon support
- **System.Drawing.Common** (8.0.0): Dynamic icon generation

## Future Enhancements

Potential improvements for future phases:
- Toast notifications (Windows 10+)
- Sound alerts
- Email notifications
- Alert history/log
- Custom alert rules
- Multiple disk drive monitoring
- Network connectivity alerts
