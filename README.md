# System Health Dashboard 📊

A real-time desktop application for comprehensive system monitoring and performance tracking.

---

## 🖥️ Project Overview

### 🎯 Project Goal

Build a real-time desktop application that monitors:

- **CPU usage**
- **Memory usage**
- **Disk activity**
- **Network activity**
- **System alerts**
- **Historical trends**

**End result:** *"This looks like a professional system monitoring tool."*

---

## 📋 Development Phases

### PHASE 0 — Scope & Vision ⚠️

> **Don't skip this.**

#### Core Principles

- Real-time but efficient
- Clean, modern UI
- Accurate metrics
- Extendable architecture

#### Target Platforms

Pick one first:

- **Windows** (recommended)
- **Linux**
- **macOS**

#### Non-Goals (for v1)

- No kernel drivers
- No cloud sync
- No admin-only features

---

### PHASE 1 — Requirements & Feature Definition

#### 🔹 Core Features (MVP)

- Live CPU usage (%)
- RAM usage (used / total)
- Disk read/write rate
- Network upload/download
- Update every 1 second

#### 🔹 UI Features

- Dashboard with cards
- Live charts
- Tray icon
- Dark mode

#### 🔹 Functional Requirements

- Runs without admin privileges
- Uses <5% CPU
- Can run in background

---

### PHASE 2 — Architecture Design

#### 🧩 High-Level Architecture

```
+---------------------+
|       UI Layer      |
|  Charts, Controls   |
+----------▲----------+
           |
+----------|----------+
|   Application Core  |
|  State, Scheduler   |
+----------▲----------+
           |
+----------|----------+
|   Metrics Providers |
|  CPU / RAM / Disk   |
+---------------------+
```

#### 🧠 Key Design Patterns

- **Observer / Pub-Sub** (metrics → UI)
- **Strategy** (OS-specific collectors)
- **Dependency Injection**
- **Thread-safe data buffers**

---

### PHASE 3 — Technology Stack ✅

**Status: COMPLETED**

**Selected Stack:**
- ✅ **C# (.NET 8)**
- ✅ **WPF** (Windows Presentation Foundation)
- ✅ **LiveCharts2** for real-time charting
- ✅ **Performance Counters** for metrics
- ✅ **Hardcodet.NotifyIcon.Wpf** for system tray

#### Option A (Low-level, impressive)

- **C++**
- **Qt** (UI + charts)
- Native OS APIs

#### Option B (Fast & clean)

- **C# (.NET)**
- **WPF or WinUI**
- Performance Counters

#### Option C (Cross-platform)

- **Rust**
- **Tauri**
- Native system libraries

> 💡 **If you want maximum "systems" credibility: C++ + Qt.**

---

### PHASE 4 — Metric Collection Layer ✅

**Status: COMPLETED**

#### 🔹 CPU Metrics

- ✅ Total usage %
- ✅ Per-core usage

**Implementation:**
- Windows: `GetSystemTimes`
- Linux: `/proc/stat`
- macOS: `host_processor_info`

#### 🔹 Memory Metrics

- Total RAM
- Used RAM
- Cache

**Implementation:**
- Windows: `GlobalMemoryStatusEx`
- Linux: `/proc/meminfo`

#### 🔹 Disk Metrics

- Read/write bytes per second
- Disk usage %

**Implementation:**
- Windows: Performance Counters
- Linux: `/proc/diskstats`

#### 🔹 Network Metrics

- Upload/download speed
- Per-interface stats

**Implementation:**
- Windows: `GetIfTable2`
- Linux: `/proc/net/dev`

#### 🔹 Metric Scheduler

- Background thread
- Fixed sampling interval
- Ring buffer for history

---

### PHASE 5 — Application Core ✅

**Status: COMPLETED**

#### Responsibilities

- ✅ Start/stop metric collectors
- ✅ Store metrics history
- ✅ Handle update frequency
- ✅ Emit events to UI

#### Internal Components

- **MetricManager**
- **SamplingScheduler**
- **MetricCache**
- **EventBus**

---

### PHASE 6 — UI/UX Design ✅

**Status: COMPLETED**

#### Dashboard Layout

```
+------------------------------+
|  CPU | RAM | Disk | Network  |
+------------------------------+
|      Live Line Chart         |
+------------------------------+
| Alerts | System Info         |
+------------------------------+
```

#### UI Elements

- ✅ Line charts (last 60 seconds)
- ✅ Progress bars
- ✅ Icons (⚙️🔥📊)
- ✅ Modern dark theme
- ✅ Real-time metric cards
- ✅ Four separate charts (CPU, Memory, Disk, Network)

#### UX Rules

- ✅ No clutter
- ✅ Consistent colors
- ✅ Smooth animations (but subtle)

---

### PHASE 7 — Alerts & Notifications ✅

**Status: COMPLETED**

#### Alert Types

- CPU > 85% for X seconds
- RAM usage > threshold
- Disk almost full

#### Features

- ✅ Configurable thresholds
- ✅ Desktop notifications
- ✅ Tray icon color changes
- ✅ Alert severity levels (Normal, Warning, Critical)
- ✅ Duration-based alerting
- ✅ Alert cooldown mechanism
- ✅ Minimize to tray functionality

**See:** [Phase 7 Implementation Details](docs/phase-7-implementation.md)

---

### PHASE 8 — Performance Optimization

#### Key Goals

- Minimal CPU usage
- No UI freezing
- Efficient sampling

#### Techniques

- Lock-free queues
- Double buffering
- Avoid UI redraws unless changed

---

### PHASE 9 — Persistence & Settings

#### Stored Settings

- Refresh rate
- Thresholds
- Theme
- Startup behavior

#### Storage

- JSON / INI file
- Stored in user directory

---

### PHASE 10 — Testing Strategy

#### Manual Tests

- Stress CPU/RAM
- Unplug network
- Fill disk

#### Automated Tests

- Metric parsing tests
- Scheduler timing tests

#### Edge Cases

- Sleep / resume
- Laptop battery mode
- VM environments

---

### PHASE 11 — Polishing ✨

#### Professional Touches

- Splash screen
- About page
- Version info
- Export system snapshot

#### Packaging

- Installer
- App icon
- Code signing (optional)

---

### PHASE 12 — Future Extensions (v2 Ideas)

- Per-process stats
- Historical graphs (hours/days)
- Plugin system
- Remote monitoring
- CSV export

---

## 🏁 Final Outcome

You'll end up with:

- ✅ A real desktop system tool
- ✅ Clear architecture
- ✅ OS-level knowledge
- ✅ CV & GitHub-ready project

---

## 📄 License

See [LICENSE](LICENSE) file for details.
