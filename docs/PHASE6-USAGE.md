# Phase 6 - UI Usage Guide

## Running the Application

### From Visual Studio
1. Set `SystemHealthDashboard.UI` as startup project
2. Press F5 or click Run

### From Command Line
```bash
cd System-Health-Dashboard
dotnet run --project src/SystemHealthDashboard.UI/SystemHealthDashboard.UI.csproj
```

### From Executable
```bash
cd src/SystemHealthDashboard.UI/bin/Debug/net8.0-windows
./SystemHealthDashboard.UI.exe
```

## Dashboard Overview

### Metric Cards (Top Row)

**🔥 CPU Card**
- Shows current CPU usage percentage
- Blue progress bar visualization
- Updates every second

**💾 Memory Card**
- Shows memory usage percentage
- Displays used/total memory in MB
- Green progress bar visualization
- Updates every second

**💿 Disk I/O Card**
- Shows read speed (orange ↓)
- Shows write speed (red ↑)
- Values in KB/s
- Updates every second

**🌐 Network Card**
- Shows download speed (purple ↓)
- Shows upload speed (pink ↑)
- Values in KB/s
- Updates every second

### Live Charts (Bottom Half)

**CPU Usage History**
- Blue line chart
- Shows last 60 seconds of CPU usage
- Auto-scales Y-axis
- Smooth line animation

**Memory Usage History**
- Green line chart
- Shows last 60 seconds of memory usage
- Auto-scales Y-axis
- Smooth line animation

**Disk I/O History**
- Orange line: Read speed
- Red line: Write speed
- Shows last 60 seconds
- Auto-scales for both metrics

**Network Traffic History**
- Purple line: Download speed
- Pink line: Upload speed
- Shows last 60 seconds
- Auto-scales for both metrics

## Understanding the Metrics

### CPU Usage
- **0-30%**: Normal (light usage)
- **30-70%**: Moderate (typical workload)
- **70-100%**: High (heavy workload or performance issue)

### Memory Usage
- **0-50%**: Healthy
- **50-80%**: Normal
- **80-95%**: High (may need more RAM)
- **95-100%**: Critical (swapping to disk)

### Disk I/O
- **< 1 MB/s**: Light usage
- **1-10 MB/s**: Moderate usage
- **> 10 MB/s**: Heavy disk activity

### Network Traffic
- **< 100 KB/s**: Light usage
- **100 KB/s - 1 MB/s**: Moderate usage
- **> 1 MB/s**: Heavy network activity

## Customization (For Developers)

### Change Update Interval

Edit `MainViewModel.cs`:
```csharp
_appCore = new ApplicationCore(
    updateIntervalMs: 500,  // Change from 1000 to 500ms
    historySize: 60
);
```

### Change History Window

Edit `MainViewModel.cs`:
```csharp
_appCore = new ApplicationCore(
    updateIntervalMs: 1000,
    historySize: 120  // Change from 60 to 120 seconds
);

private readonly int _maxDataPoints = 120; // Match history size
```

### Change Chart Colors

Edit `MainViewModel.cs`, in `InitializeCharts()`:
```csharp
Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 }
```

Available SKColors:
- `SKColors.CornflowerBlue` (default CPU)
- `SKColors.MediumSeaGreen` (default Memory)
- `SKColors.Orange` (default Disk Read)
- `SKColors.OrangeRed` (default Disk Write)
- `SKColors.Purple` (default Network Download)
- `SKColors.DeepPink` (default Network Upload)
- And many more...

### Change Window Size

Edit `MainWindow.xaml`:
```xml
<Window ... Height="800" Width="1400">
```

### Change Theme Colors

Edit `MainWindow.xaml`:
```xml
Background="#FF1E1E1E"  <!-- Window background -->

<Style TargetType="Border" x:Key="CardStyle">
    <Setter Property="Background" Value="#FF2D2D30"/>  <!-- Card background -->
</Style>
```

### Disable Specific Charts

Comment out unwanted charts in `MainWindow.xaml`:
```xml
<!-- CPU Chart -->
<!--
<Border Grid.Row="0" Grid.Column="0" Style="{StaticResource CardStyle}">
    ...
</Border>
-->
```

## Troubleshooting

### Application doesn't start
- Ensure .NET 8.0 SDK is installed
- Build solution first: `dotnet build`
- Check for build errors

### Charts not updating
- Check if ApplicationCore is started
- Verify event subscriptions in MainViewModel
- Check for Dispatcher issues

### High CPU usage from dashboard
- Reduce update frequency (increase `updateIntervalMs`)
- Reduce history size (decrease `historySize`)
- Check for event handler leaks

### Memory leak
- Ensure `MainViewModel.Dispose()` is called on window close
- Verify all event handlers are unsubscribed
- Check `ApplicationCore` is properly disposed

### Charts not smooth
- Ensure SkiaSharp is using GPU acceleration
- Check graphics drivers are up to date
- Reduce number of data points displayed

## Keyboard Shortcuts

Currently, no keyboard shortcuts implemented. Could add:
- **F11**: Toggle fullscreen
- **Ctrl+R**: Reset/clear history
- **Ctrl+E**: Export data
- **Ctrl+S**: Settings
- **ESC**: Exit fullscreen

## Tips & Tricks

1. **Monitor Specific Metrics**: Watch the charts for patterns
2. **Identify Spikes**: Sudden spikes indicate system events
3. **Baseline Normal**: Learn your system's normal ranges
4. **Correlate Metrics**: High CPU often means high disk I/O
5. **Memory Trends**: Gradual increase may indicate memory leak

## Known Issues

1. First data point may show invalid values (initializing)
2. Charts may flicker on first update (LiveCharts initialization)
3. Window resizing may briefly distort charts

## Support

For issues or questions:
1. Check `docs/PHASE6-COMPLETE.md` for implementation details
2. Review `MainViewModel.cs` for data flow
3. Check LiveCharts2 documentation for chart customization

## Performance Tips

- Keep history size at 60 or less for optimal performance
- Use 1000ms update interval (1 second) for balance
- Close unnecessary applications while monitoring
- Run as Administrator for accurate metrics (Windows)

## Advanced Usage

### Multiple Windows
You can launch multiple instances to monitor different aspects:
```bash
# Instance 1: Default view
./SystemHealthDashboard.UI.exe

# Instance 2: Could be customized for network only
./SystemHealthDashboard.UI.exe
```

### Integration with Other Tools
- Export data via copy-paste from metric values
- Screenshot charts for reports
- Use alongside Windows Task Manager for detailed analysis

---

**Version**: 1.0  
**Last Updated**: January 23, 2026  
**Platform**: Windows (.NET 8.0)
