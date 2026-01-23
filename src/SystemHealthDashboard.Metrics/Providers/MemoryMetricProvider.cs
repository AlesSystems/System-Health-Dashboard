using System.Runtime.InteropServices;
using SystemHealthDashboard.Metrics.Interfaces;
using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.Metrics.Providers;

public class MemoryMetricProvider : IMetricProvider<MemoryMetricData>
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;

        public MEMORYSTATUSEX()
        {
            dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        }
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

    public void Initialize()
    {
        // No initialization needed
    }

    public MemoryMetricData GetCurrentMetric()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return GetWindowsMemoryMetric();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return GetLinuxMemoryMetric();
        }
        else
        {
            return new MemoryMetricData(0, 0, 0);
        }
    }

    private MemoryMetricData GetWindowsMemoryMetric()
    {
        var memStatus = new MEMORYSTATUSEX();
        if (GlobalMemoryStatusEx(memStatus))
        {
            long total = (long)memStatus.ullTotalPhys;
            long available = (long)memStatus.ullAvailPhys;
            long used = total - available;

            return new MemoryMetricData(total, used, available);
        }

        return new MemoryMetricData(0, 0, 0);
    }

    private MemoryMetricData GetLinuxMemoryMetric()
    {
        try
        {
            var lines = File.ReadAllLines("/proc/meminfo");
            long total = 0;
            long available = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("MemTotal:"))
                {
                    total = ParseMemInfoLine(line) * 1024;
                }
                else if (line.StartsWith("MemAvailable:"))
                {
                    available = ParseMemInfoLine(line) * 1024;
                }
            }

            long used = total - available;
            return new MemoryMetricData(total, used, available);
        }
        catch
        {
            return new MemoryMetricData(0, 0, 0);
        }
    }

    private long ParseMemInfoLine(string line)
    {
        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2 && long.TryParse(parts[1], out long value))
        {
            return value;
        }
        return 0;
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}
