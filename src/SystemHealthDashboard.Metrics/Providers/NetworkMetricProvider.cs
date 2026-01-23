using System.Diagnostics;
using SystemHealthDashboard.Metrics.Interfaces;
using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.Metrics.Providers;

public class NetworkMetricProvider : IMetricProvider<NetworkMetricData>
{
    private PerformanceCounter _downloadCounter;
    private PerformanceCounter _uploadCounter;
    private string _networkInterfaceName;

    public void Initialize()
    {
        _networkInterfaceName = GetActiveNetworkInterface();
        
        if (!string.IsNullOrEmpty(_networkInterfaceName))
        {
            _downloadCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", _networkInterfaceName);
            _uploadCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", _networkInterfaceName);

            _downloadCounter.NextValue();
            _uploadCounter.NextValue();
        }
    }

    private string GetActiveNetworkInterface()
    {
        try
        {
            var category = new PerformanceCounterCategory("Network Interface");
            var instanceNames = category.GetInstanceNames();

            foreach (var name in instanceNames)
            {
                if (!name.Contains("Loopback") && 
                    !name.Contains("isatap") && 
                    !name.Contains("Teredo"))
                {
                    return name;
                }
            }

            return instanceNames.Length > 0 ? instanceNames[0] : null;
        }
        catch
        {
            return null;
        }
    }

    public NetworkMetricData GetCurrentMetric()
    {
        if (_downloadCounter == null || _uploadCounter == null)
        {
            Initialize();
        }

        if (_downloadCounter == null || _uploadCounter == null)
        {
            return new NetworkMetricData(0, 0);
        }

        long downloadBytes = (long)_downloadCounter.NextValue();
        long uploadBytes = (long)_uploadCounter.NextValue();

        return new NetworkMetricData(downloadBytes, uploadBytes);
    }

    public void Dispose()
    {
        _downloadCounter?.Dispose();
        _uploadCounter?.Dispose();
    }
}
