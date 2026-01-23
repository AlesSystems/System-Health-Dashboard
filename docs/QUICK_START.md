# Phase 7 - Quick Start Guide

## Running the Application

### Console Demo (Recommended for Testing Alerts)

The console demo has lowered alert thresholds for easier testing:

```bash
cd src\SystemHealthDashboard.Demo
dotnet run --configuration Release
```

**Features demonstrated:**
- Real-time metric monitoring
- Alert triggering (CPU threshold: 50%, Memory threshold: 60%)
- Alert severity changes
- Console-based notifications with colors
- Metric history display

**Expected output:**
- Live metric updates every second
- Alert messages when thresholds are exceeded
- Severity change notifications
- Final metric history summary

### WPF UI Application

To run the full graphical dashboard:

```bash
cd src\SystemHealthDashboard.UI
dotnet run --configuration Release
```

**Features:**
- Real-time dashboard with charts
- Four metric cards (CPU, Memory, Disk, Network)
- Live line charts for historical data
- System tray icon with color-coded status
  - Green = Normal
  - Orange = Warning (High CPU/Memory)
  - Red = Critical (Disk almost full)
- Desktop notifications when alerts trigger
- Minimize to tray functionality

**UI Interactions:**
- Minimize window → automatically goes to system tray
- Left-click tray icon → restore window
- Right-click tray icon → context menu (Show Dashboard, Exit)
- Close button → minimizes to tray (doesn't exit)

## Building the Project

### Build All Projects

```bash
dotnet build SystemHealthDashboard.sln --configuration Release
```

### Build Individual Projects

```bash
# Core library
dotnet build src\SystemHealthDashboard.Core\SystemHealthDashboard.Core.csproj --configuration Release

# Metrics library
dotnet build src\SystemHealthDashboard.Metrics\SystemHealthDashboard.Metrics.csproj --configuration Release

# UI application
dotnet build src\SystemHealthDashboard.UI\SystemHealthDashboard.UI.csproj --configuration Release

# Console demo
dotnet build src\SystemHealthDashboard.Demo\SystemHealthDashboard.Demo.csproj --configuration Release
```

## Testing Alert System

### Manual Alert Triggering

To test alerts without waiting:

1. **CPU Alert:**
   - Run a CPU-intensive task
   - Open multiple browser tabs
   - Run a video encoding process

2. **Memory Alert:**
   - Open many applications
   - Load large files in memory
   - Create a memory leak test program

3. **Disk Alert:**
   - Fill up a drive to >90%
   - (Requires actual disk space usage)

### Lowering Thresholds for Testing

Edit the demo or UI code to lower thresholds:

```csharp
app.Alerts.Configuration.CpuThresholdPercent = 30.0;       // Alert at 30% CPU
app.Alerts.Configuration.CpuThresholdDurationSeconds = 3;  // After 3 seconds
app.Alerts.Configuration.MemoryThresholdPercent = 40.0;    // Alert at 40% Memory
```

## Project Structure

```
System-Health-Dashboard/
├── src/
│   ├── SystemHealthDashboard.Core/
│   │   ├── Events/              # Event bus and event args
│   │   ├── Models/              # Alert configuration models
│   │   └── Services/            # AlertService, ApplicationCore, MetricCache
│   │
│   ├── SystemHealthDashboard.Metrics/
│   │   ├── Interfaces/          # IMetricProvider
│   │   ├── Models/              # Metric data models
│   │   ├── Providers/           # CPU, Memory, Disk, Network providers
│   │   └── Schedulers/          # Metric scheduling and ring buffer
│   │
│   ├── SystemHealthDashboard.UI/
│   │   ├── Helpers/             # TrayIconHelper
│   │   ├── Services/            # NotificationService
│   │   ├── ViewModels/          # MainViewModel, ViewModelBase
│   │   ├── App.xaml             # Application with tray icon
│   │   └── MainWindow.xaml      # Main dashboard window
│   │
│   └── SystemHealthDashboard.Demo/
│       └── Program.cs           # Console demo application
│
└── docs/
    ├── phase-7-implementation.md    # Detailed Phase 7 docs
    └── PHASE_7_SUMMARY.md          # Implementation summary
```

## Key Files Modified in Phase 7

### New Files (Alert System):
- `Core/Models/AlertConfiguration.cs`
- `Core/Services/AlertService.cs`
- `UI/Services/NotificationService.cs`
- `UI/Helpers/TrayIconHelper.cs`

### Modified Files:
- `Core/Services/ApplicationCore.cs` - Integrated AlertService
- `UI/ViewModels/MainViewModel.cs` - Added alert event handling
- `UI/App.xaml` / `App.xaml.cs` - Added system tray support
- `UI/MainWindow.xaml.cs` - Added minimize to tray
- `Demo/Program.cs` - Added alert demonstration

## Dependencies

The project uses:
- **.NET 8.0**
- **LiveChartsCore.SkiaSharpView.WPF** (2.0.0-rc2) - Charts
- **Hardcodet.NotifyIcon.Wpf** (1.1.0) - System tray icon
- **System.Drawing.Common** (8.0.0) - Icon generation
- **System.Diagnostics.PerformanceCounter** - Windows performance counters

## Next Steps

After Phase 7, the next phases would be:
- **Phase 8:** Performance Optimization
- **Phase 9:** Persistence & Settings
- **Phase 10:** Testing Strategy
- **Phase 11:** Polishing
- **Phase 12:** Future Extensions

## Troubleshooting

### Application won't start
- Ensure .NET 8.0 SDK is installed
- Run `dotnet restore` before building
- Check Windows Performance Counters are accessible

### Alerts not triggering
- Check threshold configuration
- Ensure duration requirement is met (sustained high usage)
- Check cooldown period hasn't been activated
- Verify notifications are enabled in configuration

### Tray icon not showing
- Ensure Hardcodet.NotifyIcon.Wpf package is installed
- Check Windows system tray settings
- Verify icon is being created properly

### Charts not displaying
- Ensure LiveChartsCore packages are installed
- Check that metrics are being collected
- Verify UI thread is updating properly
