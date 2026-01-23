using System.Collections.ObjectModel;
using System.Windows;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SystemHealthDashboard.Core.Services;
using SystemHealthDashboard.Core.Models;
using SystemHealthDashboard.Metrics.Models;
using SystemHealthDashboard.UI.Services;
using SystemHealthDashboard.UI.Helpers;
using SystemHealthDashboard.UI.Models;

namespace SystemHealthDashboard.UI.ViewModels;

public class MainViewModel : ViewModelBase, IDisposable
{
    private readonly ApplicationCore _appCore;
    private readonly NotificationService _notificationService;
    private readonly UIUpdateThrottler _uiThrottler;
    private readonly MetricUpdateBatch _pendingUpdates;
    private readonly int _maxDataPoints = 60;

    private readonly ObservableCollection<ObservableValue> _cpuValues;
    private readonly ObservableCollection<ObservableValue> _memoryValues;
    private readonly ObservableCollection<ObservableValue> _diskReadValues;
    private readonly ObservableCollection<ObservableValue> _diskWriteValues;
    private readonly ObservableCollection<ObservableValue> _networkDownloadValues;
    private readonly ObservableCollection<ObservableValue> _networkUploadValues;

    private double _currentCpu;
    private double _currentMemory;
    private long _currentMemoryUsedMB;
    private long _currentMemoryTotalMB;
    private double _currentDiskRead;
    private double _currentDiskWrite;
    private double _currentNetworkDownload;
    private double _currentNetworkUpload;
    private AlertSeverity _currentAlertSeverity;

    public event EventHandler<AlertSeverity>? AlertSeverityChanged;

    public MainViewModel()
    {
        _appCore = new ApplicationCore(updateIntervalMs: 1000, historySize: 60);
        _notificationService = new NotificationService(_appCore.Alerts.Configuration);
        _uiThrottler = new UIUpdateThrottler(intervalMs: 100);
        _pendingUpdates = new MetricUpdateBatch();

        _cpuValues = new ObservableCollection<ObservableValue>();
        _memoryValues = new ObservableCollection<ObservableValue>();
        _diskReadValues = new ObservableCollection<ObservableValue>();
        _diskWriteValues = new ObservableCollection<ObservableValue>();
        _networkDownloadValues = new ObservableCollection<ObservableValue>();
        _networkUploadValues = new ObservableCollection<ObservableValue>();

        InitializeCharts();
        SubscribeToEvents();
        _appCore.Start();
    }

    private void InitializeCharts()
    {
        CpuSeries = new ISeries[]
        {
            new LineSeries<ObservableValue>
            {
                Values = _cpuValues,
                Name = "CPU %",
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 2 },
                GeometrySize = 0
            }
        };

        MemorySeries = new ISeries[]
        {
            new LineSeries<ObservableValue>
            {
                Values = _memoryValues,
                Name = "Memory %",
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.MediumSeaGreen) { StrokeThickness = 2 },
                GeometrySize = 0
            }
        };

        DiskSeries = new ISeries[]
        {
            new LineSeries<ObservableValue>
            {
                Values = _diskReadValues,
                Name = "Read KB/s",
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 2 },
                GeometrySize = 0
            },
            new LineSeries<ObservableValue>
            {
                Values = _diskWriteValues,
                Name = "Write KB/s",
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.OrangeRed) { StrokeThickness = 2 },
                GeometrySize = 0
            }
        };

        NetworkSeries = new ISeries[]
        {
            new LineSeries<ObservableValue>
            {
                Values = _networkDownloadValues,
                Name = "Download KB/s",
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.Purple) { StrokeThickness = 2 },
                GeometrySize = 0
            },
            new LineSeries<ObservableValue>
            {
                Values = _networkUploadValues,
                Name = "Upload KB/s",
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.DeepPink) { StrokeThickness = 2 },
                GeometrySize = 0
            }
        };
    }

    private void SubscribeToEvents()
    {
        _appCore.EventBus.CpuMetricReceived += (s, e) =>
        {
            lock (_pendingUpdates)
            {
                _pendingUpdates.CpuMetric = e.Metric;
                _pendingUpdates.HasCpuUpdate = true;
            }
            
            _uiThrottler.ScheduleUpdate("cpu", () =>
            {
                CpuMetricData? metric;
                lock (_pendingUpdates)
                {
                    if (!_pendingUpdates.HasCpuUpdate) return;
                    metric = _pendingUpdates.CpuMetric;
                    _pendingUpdates.HasCpuUpdate = false;
                }
                
                if (metric != null)
                {
                    CurrentCpu = metric.TotalUsagePercent;
                    AddDataPoint(_cpuValues, metric.TotalUsagePercent);
                }
            });
        };

        _appCore.EventBus.MemoryMetricReceived += (s, e) =>
        {
            lock (_pendingUpdates)
            {
                _pendingUpdates.MemoryMetric = e.Metric;
                _pendingUpdates.HasMemoryUpdate = true;
            }
            
            _uiThrottler.ScheduleUpdate("memory", () =>
            {
                MemoryMetricData? metric;
                lock (_pendingUpdates)
                {
                    if (!_pendingUpdates.HasMemoryUpdate) return;
                    metric = _pendingUpdates.MemoryMetric;
                    _pendingUpdates.HasMemoryUpdate = false;
                }
                
                if (metric != null)
                {
                    CurrentMemory = metric.UsagePercent;
                    CurrentMemoryUsedMB = metric.UsedBytes / 1024 / 1024;
                    CurrentMemoryTotalMB = metric.TotalBytes / 1024 / 1024;
                    AddDataPoint(_memoryValues, metric.UsagePercent);
                }
            });
        };

        _appCore.EventBus.DiskMetricReceived += (s, e) =>
        {
            lock (_pendingUpdates)
            {
                _pendingUpdates.DiskMetric = e.Metric;
                _pendingUpdates.HasDiskUpdate = true;
            }
            
            _uiThrottler.ScheduleUpdate("disk", () =>
            {
                DiskMetricData? metric;
                lock (_pendingUpdates)
                {
                    if (!_pendingUpdates.HasDiskUpdate) return;
                    metric = _pendingUpdates.DiskMetric;
                    _pendingUpdates.HasDiskUpdate = false;
                }
                
                if (metric != null)
                {
                    CurrentDiskRead = metric.ReadBytesPerSecond / 1024.0;
                    CurrentDiskWrite = metric.WriteBytesPerSecond / 1024.0;
                    AddDataPoint(_diskReadValues, CurrentDiskRead);
                    AddDataPoint(_diskWriteValues, CurrentDiskWrite);
                }
            });
        };

        _appCore.EventBus.NetworkMetricReceived += (s, e) =>
        {
            lock (_pendingUpdates)
            {
                _pendingUpdates.NetworkMetric = e.Metric;
                _pendingUpdates.HasNetworkUpdate = true;
            }
            
            _uiThrottler.ScheduleUpdate("network", () =>
            {
                NetworkMetricData? metric;
                lock (_pendingUpdates)
                {
                    if (!_pendingUpdates.HasNetworkUpdate) return;
                    metric = _pendingUpdates.NetworkMetric;
                    _pendingUpdates.HasNetworkUpdate = false;
                }
                
                if (metric != null)
                {
                    CurrentNetworkDownload = metric.DownloadBytesPerSecond / 1024.0;
                    CurrentNetworkUpload = metric.UploadBytesPerSecond / 1024.0;
                    AddDataPoint(_networkDownloadValues, CurrentNetworkDownload);
                    AddDataPoint(_networkUploadValues, CurrentNetworkUpload);
                }
            });
        };

        _appCore.Alerts.AlertTriggered += (s, alert) =>
        {
            _notificationService.ShowAlert(alert);
        };

        _appCore.Alerts.SeverityChanged += (s, severity) =>
        {
            _uiThrottler.ScheduleUpdate("severity", () =>
            {
                CurrentAlertSeverity = severity;
                AlertSeverityChanged?.Invoke(this, severity);
            });
        };
    }

    private void AddDataPoint(ObservableCollection<ObservableValue> collection, double value)
    {
        collection.Add(new ObservableValue(value));
        if (collection.Count > _maxDataPoints)
        {
            collection.RemoveAt(0);
        }
    }

    public ISeries[] CpuSeries { get; private set; } = Array.Empty<ISeries>();
    public ISeries[] MemorySeries { get; private set; } = Array.Empty<ISeries>();
    public ISeries[] DiskSeries { get; private set; } = Array.Empty<ISeries>();
    public ISeries[] NetworkSeries { get; private set; } = Array.Empty<ISeries>();

    public double CurrentCpu
    {
        get => _currentCpu;
        set => SetProperty(ref _currentCpu, value);
    }

    public double CurrentMemory
    {
        get => _currentMemory;
        set => SetProperty(ref _currentMemory, value);
    }

    public long CurrentMemoryUsedMB
    {
        get => _currentMemoryUsedMB;
        set => SetProperty(ref _currentMemoryUsedMB, value);
    }

    public long CurrentMemoryTotalMB
    {
        get => _currentMemoryTotalMB;
        set => SetProperty(ref _currentMemoryTotalMB, value);
    }

    public double CurrentDiskRead
    {
        get => _currentDiskRead;
        set => SetProperty(ref _currentDiskRead, value);
    }

    public double CurrentDiskWrite
    {
        get => _currentDiskWrite;
        set => SetProperty(ref _currentDiskWrite, value);
    }

    public double CurrentNetworkDownload
    {
        get => _currentNetworkDownload;
        set => SetProperty(ref _currentNetworkDownload, value);
    }

    public double CurrentNetworkUpload
    {
        get => _currentNetworkUpload;
        set => SetProperty(ref _currentNetworkUpload, value);
    }

    public AlertSeverity CurrentAlertSeverity
    {
        get => _currentAlertSeverity;
        set => SetProperty(ref _currentAlertSeverity, value);
    }

    public void Dispose()
    {
        _uiThrottler?.Dispose();
        _appCore?.Dispose();
    }
}
