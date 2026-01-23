using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.Metrics.Interfaces;

public interface IMetricProvider<T> where T : MetricData
{
    T GetCurrentMetric();
    void Initialize();
    void Dispose();
}
