# Phase 5 - Application Core Usage Guide

## Quick Start

```csharp
using SystemHealthDashboard.Core.Services;

// Create and configure ApplicationCore
var app = new ApplicationCore(
    updateIntervalMs: 1000,  // Update every second
    historySize: 60           // Keep 60 samples in history
);

// Subscribe to events
app.EventBus.CpuMetricReceived += (sender, e) => 
{
    var metric = e.Metric;
    Console.WriteLine($"CPU: {metric.TotalUsagePercent:F2}%");
};

app.EventBus.MemoryMetricReceived += (sender, e) => 
{
    var metric = e.Metric;
    Console.WriteLine($"Memory: {metric.UsagePercent:F2}%");
};

// Start collecting metrics
app.Start();

// ... application runs ...

// Stop collecting
app.Stop();

// Get current metrics
var currentCpu = app.GetCurrentCpuMetric();
var currentMemory = app.GetCurrentMemoryMetric();

// Get history
var cpuHistory = app.GetCpuHistory();
var memoryHistory = app.GetMemoryHistory();

// Cleanup
app.Dispose();
```

## Architecture Overview

```
ApplicationCore
    │
    ├── EventBus (event distribution)
    │   ├── CpuMetricReceived
    │   ├── MemoryMetricReceived
    │   ├── DiskMetricReceived
    │   └── NetworkMetricReceived
    │
    ├── MetricCache (current + history)
    │   ├── Current metrics
    │   └── History buffer
    │
    └── MetricManager (from Phase 4)
        └── Metric Providers
```

## API Reference

### ApplicationCore Class

**Constructor**
```csharp
ApplicationCore(int updateIntervalMs = 1000, int historySize = 60)
```

**Properties**
- `EventBus EventBus { get; }` - Event distribution system
- `MetricCache Cache { get; }` - Direct cache access
- `bool IsRunning { get; }` - Collection status

**Methods**
- `void Start()` - Start metric collection
- `void Stop()` - Stop metric collection
- `CpuMetricData? GetCurrentCpuMetric()` - Get latest CPU metric
- `MemoryMetricData? GetCurrentMemoryMetric()` - Get latest memory metric
- `DiskMetricData? GetCurrentDiskMetric()` - Get latest disk metric
- `NetworkMetricData? GetCurrentNetworkMetric()` - Get latest network metric
- `IReadOnlyList<T> GetCpuHistory()` - Get CPU history
- `IReadOnlyList<T> GetMemoryHistory()` - Get memory history
- `IReadOnlyList<T> GetDiskHistory()` - Get disk history
- `IReadOnlyList<T> GetNetworkHistory()` - Get network history
- `void Dispose()` - Cleanup resources

### EventBus Class

**Events**
```csharp
event EventHandler<CpuMetricEventArgs>? CpuMetricReceived;
event EventHandler<MemoryMetricEventArgs>? MemoryMetricReceived;
event EventHandler<DiskMetricEventArgs>? DiskMetricReceived;
event EventHandler<NetworkMetricEventArgs>? NetworkMetricReceived;
```

### MetricEventArgs Classes

All inherit from `MetricEventArgs`:
- `CpuMetricEventArgs` - Contains `CpuMetricData Metric`
- `MemoryMetricEventArgs` - Contains `MemoryMetricData Metric`
- `DiskMetricEventArgs` - Contains `DiskMetricData Metric`
- `NetworkMetricEventArgs` - Contains `NetworkMetricData Metric`

Each has:
- `Metric` property - Strongly typed metric data
- `Timestamp` property - When the metric was collected

## Usage Patterns

### Pattern 1: Real-time Monitoring

```csharp
var app = new ApplicationCore(1000, 60);

app.EventBus.CpuMetricReceived += UpdateCpuDisplay;
app.EventBus.MemoryMetricReceived += UpdateMemoryDisplay;

app.Start();
```

### Pattern 2: Historical Analysis

```csharp
app.Stop();

var history = app.GetCpuHistory();
var avgCpu = history.Average(m => m.TotalUsagePercent);
var maxCpu = history.Max(m => m.TotalUsagePercent);
```

### Pattern 3: Polling Current State

```csharp
app.Start();

// In a timer or update loop:
var cpu = app.GetCurrentCpuMetric();
if (cpu != null && cpu.TotalUsagePercent > 80)
{
    Alert("High CPU usage!");
}
```

## Thread Safety

All public methods are thread-safe:
- MetricCache uses locks for concurrent access
- EventBus event invocation is thread-safe
- ApplicationCore properly synchronizes state

## Best Practices

1. **Subscribe before Start()** - Ensure event handlers are attached
2. **Always Dispose()** - Clean up resources when done
3. **Check for null** - Current metrics may be null initially
4. **Use appropriate intervals** - Balance responsiveness vs. overhead
5. **Limit history size** - More history = more memory usage

## Example: WPF Integration

```csharp
public class MainViewModel : IDisposable
{
    private ApplicationCore _app;
    
    public MainViewModel()
    {
        _app = new ApplicationCore(1000, 60);
        
        _app.EventBus.CpuMetricReceived += (s, e) => 
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CpuUsage = e.Metric.TotalUsagePercent;
            });
        };
        
        _app.Start();
    }
    
    public void Dispose() => _app?.Dispose();
}
```

## Troubleshooting

**Issue**: No events firing
- Check if `Start()` was called
- Verify event handlers are subscribed
- Check `IsRunning` property

**Issue**: Null metrics
- Wait for first collection cycle (interval time)
- Check if metric providers initialized correctly

**Issue**: High memory usage
- Reduce `historySize` parameter
- Consider clearing cache periodically with `Cache.Clear()`
