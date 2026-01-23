# PHASE 4 — Metric Collection Layer

## 🔹 CPU Metrics

- Total usage %
- Per-core usage

**Implementation:**
- Windows: GetSystemTimes
- Linux: /proc/stat
- macOS: host_processor_info

## 🔹 Memory Metrics

- Total RAM
- Used RAM
- Cache

**Implementation:**
- Windows: GlobalMemoryStatusEx
- Linux: /proc/meminfo

## 🔹 Disk Metrics

- Read/write bytes per second
- Disk usage %

**Implementation:**
- Windows: Performance Counters
- Linux: /proc/diskstats

## 🔹 Network Metrics

- Upload/download speed
- Per-interface stats

**Implementation:**
- Windows: GetIfTable2
- Linux: /proc/net/dev

## 🔹 Metric Scheduler

- Background thread
- Fixed sampling interval
- Ring buffer for history
