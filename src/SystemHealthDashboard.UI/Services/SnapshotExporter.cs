using System.IO;
using System.Text;
using System.Text.Json;
using SystemHealthDashboard.Metrics.Models;

namespace SystemHealthDashboard.UI.Services;

public class SnapshotExporter
{
    public class SystemSnapshot
    {
        public DateTime Timestamp { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public int ProcessorCount { get; set; }
        public CpuMetricData? CurrentCpu { get; set; }
        public MemoryMetricData? CurrentMemory { get; set; }
        public DiskMetricData? CurrentDisk { get; set; }
        public NetworkMetricData? CurrentNetwork { get; set; }
        public List<CpuMetricData>? CpuHistory { get; set; }
        public List<MemoryMetricData>? MemoryHistory { get; set; }
        public List<DiskMetricData>? DiskHistory { get; set; }
        public List<NetworkMetricData>? NetworkHistory { get; set; }
    }

    public static SystemSnapshot CreateSnapshot(
        CpuMetricData? cpu,
        MemoryMetricData? memory,
        DiskMetricData? disk,
        NetworkMetricData? network,
        IReadOnlyList<CpuMetricData>? cpuHistory = null,
        IReadOnlyList<MemoryMetricData>? memoryHistory = null,
        IReadOnlyList<DiskMetricData>? diskHistory = null,
        IReadOnlyList<NetworkMetricData>? networkHistory = null)
    {
        return new SystemSnapshot
        {
            Timestamp = DateTime.Now,
            MachineName = Environment.MachineName,
            OperatingSystem = Environment.OSVersion.ToString(),
            ProcessorCount = Environment.ProcessorCount,
            CurrentCpu = cpu,
            CurrentMemory = memory,
            CurrentDisk = disk,
            CurrentNetwork = network,
            CpuHistory = cpuHistory?.ToList(),
            MemoryHistory = memoryHistory?.ToList(),
            DiskHistory = diskHistory?.ToList(),
            NetworkHistory = networkHistory?.ToList()
        };
    }

    public static string ExportToJson(SystemSnapshot snapshot)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return JsonSerializer.Serialize(snapshot, options);
    }

    public static string ExportToText(SystemSnapshot snapshot)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("=== SYSTEM HEALTH SNAPSHOT ===");
        sb.AppendLine($"Timestamp: {snapshot.Timestamp:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Machine Name: {snapshot.MachineName}");
        sb.AppendLine($"Operating System: {snapshot.OperatingSystem}");
        sb.AppendLine($"Processor Count: {snapshot.ProcessorCount}");
        sb.AppendLine();
        
        if (snapshot.CurrentCpu != null)
        {
            sb.AppendLine("--- CPU ---");
            sb.AppendLine($"Total Usage: {snapshot.CurrentCpu.TotalUsagePercent:F2}%");
            sb.AppendLine($"Per-Core Usage: {string.Join(", ", snapshot.CurrentCpu.PerCoreUsage.Select(c => $"{c:F1}%"))}");
            sb.AppendLine();
        }
        
        if (snapshot.CurrentMemory != null)
        {
            sb.AppendLine("--- MEMORY ---");
            sb.AppendLine($"Total: {snapshot.CurrentMemory.TotalBytes / (1024.0 * 1024 * 1024):F2} GB");
            sb.AppendLine($"Used: {snapshot.CurrentMemory.UsedBytes / (1024.0 * 1024 * 1024):F2} GB");
            sb.AppendLine($"Available: {snapshot.CurrentMemory.AvailableBytes / (1024.0 * 1024 * 1024):F2} GB");
            sb.AppendLine($"Usage: {snapshot.CurrentMemory.UsagePercent:F2}%");
            sb.AppendLine();
        }
        
        if (snapshot.CurrentDisk != null)
        {
            sb.AppendLine("--- DISK I/O ---");
            sb.AppendLine($"Read: {snapshot.CurrentDisk.ReadBytesPerSecond / 1024.0:F2} KB/s");
            sb.AppendLine($"Write: {snapshot.CurrentDisk.WriteBytesPerSecond / 1024.0:F2} KB/s");
            sb.AppendLine();
        }
        
        if (snapshot.CurrentNetwork != null)
        {
            sb.AppendLine("--- NETWORK ---");
            sb.AppendLine($"Download: {snapshot.CurrentNetwork.DownloadBytesPerSecond / 1024.0:F2} KB/s");
            sb.AppendLine($"Upload: {snapshot.CurrentNetwork.UploadBytesPerSecond / 1024.0:F2} KB/s");
            sb.AppendLine();
        }
        
        return sb.ToString();
    }

    public static void SaveToFile(string content, string filePath)
    {
        File.WriteAllText(filePath, content);
    }
}
