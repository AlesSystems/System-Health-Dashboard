using System.Diagnostics;
using SystemHealthDashboard.Metrics.Interfaces;
using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.Metrics.Providers;

public class CpuMetricProvider : IMetricProvider<CpuMetricData>
{
    private PerformanceCounter _totalCpuCounter;
    private List<PerformanceCounter> _perCoreCounters;
    private int _processorCount;

    public void Initialize()
    {
        _processorCount = Environment.ProcessorCount;
        
        _totalCpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        
        _perCoreCounters = new List<PerformanceCounter>();
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
        
        List<double> perCoreUsage = new List<double>();
        foreach (var counter in _perCoreCounters)
        {
            perCoreUsage.Add(counter.NextValue());
        }

        return new CpuMetricData(totalUsage, perCoreUsage);
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
