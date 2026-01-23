using System.Diagnostics;
using SystemHealthDashboard.Metrics.Interfaces;
using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.Metrics.Providers;

public class CpuMetricProvider : IMetricProvider<CpuMetricData>
{
    private PerformanceCounter _totalCpuCounter;
    private List<PerformanceCounter> _perCoreCounters;
    private int _processorCount;
    private readonly List<double> _perCoreUsageCache;

    public CpuMetricProvider()
    {
        _perCoreUsageCache = new List<double>();
    }

    public void Initialize()
    {
        _processorCount = Environment.ProcessorCount;
        
        _totalCpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        
        _perCoreCounters = new List<PerformanceCounter>(_processorCount);
        for (int i = 0; i < _processorCount; i++)
        {
            _perCoreCounters.Add(new PerformanceCounter("Processor", "% Processor Time", i.ToString()));
        }

        _totalCpuCounter.NextValue();
        foreach (var counter in _perCoreCounters)
        {
            counter.NextValue();
        }
    }

    public CpuMetricData GetCurrentMetric()
    {
        if (_totalCpuCounter == null)
        {
            Initialize();
        }

        double totalUsage = _totalCpuCounter.NextValue();
        
        _perCoreUsageCache.Clear();
        foreach (var counter in _perCoreCounters)
        {
            _perCoreUsageCache.Add(counter.NextValue());
        }

        return new CpuMetricData(totalUsage, new List<double>(_perCoreUsageCache));
    }

    public void Dispose()
    {
        _totalCpuCounter?.Dispose();
        if (_perCoreCounters != null)
        {
            foreach (var counter in _perCoreCounters)
            {
                counter?.Dispose();
            }
        }
    }
}
