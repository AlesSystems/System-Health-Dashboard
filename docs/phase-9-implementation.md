# Phase 9 Implementation - Persistence & Settings

## Overview
Phase 9 has been successfully implemented, adding persistent settings storage and a user interface for configuring the application.

## Features Implemented

### 1. Settings Model (`ApplicationSettings.cs`)
A comprehensive settings model that includes:
- **Performance Settings**
  - `RefreshIntervalMs`: Controls how often metrics are updated (100-5000 ms)
  - `HistorySize`: Number of data points to keep in history (30-300 points)

- **Threshold Settings** (`ThresholdSettings`)
  - `CpuThresholdPercent`: CPU usage alert threshold (50-100%)
  - `MemoryThresholdPercent`: Memory usage alert threshold (50-100%)
  - `DiskUsageThresholdPercent`: Disk usage alert threshold (50-100%)
  - `CpuThresholdDurationSeconds`: How long CPU must be high before alerting
  - `MemoryThresholdDurationSeconds`: How long memory must be high before alerting
  - `NotificationsEnabled`: Enable/disable desktop notifications
  - `TrayIconColorChangeEnabled`: Enable/disable tray icon color changes on alerts

- **Theme Settings** (`ThemeSettings`)
  - `Theme`: Application theme (currently "Dark")
  - `AccentColor`: Primary accent color

- **Startup Settings** (`StartupSettings`)
  - `StartMinimized`: Start application in minimized state
  - `StartWithWindows`: Launch on Windows startup (future implementation)
  - `MinimizeToTray`: Hide window when minimized

### 2. Settings Service (`SettingsService.cs`)
Handles persistence of settings to disk:
- **Storage Location**: `%AppData%\SystemHealthDashboard\settings.json`
- **Format**: JSON with indentation for readability
- **Methods**:
  - `LoadSettings()`: Load settings from disk (returns defaults if file doesn't exist)
  - `SaveSettings()`: Save settings to disk
  - `GetCurrentSettings()`: Get current in-memory settings
  - `UpdateThresholds()`: Update and save threshold settings
  - `UpdateTheme()`: Update and save theme settings
  - `UpdateStartup()`: Update and save startup settings
  - `UpdateRefreshInterval()`: Update and save refresh interval

### 3. Settings Window (`SettingsWindow.xaml`)
A user-friendly settings interface with:
- **Performance Section**
  - Refresh Interval slider (100-5000 ms)
  - History Size slider (30-300 points)

- **Alert Thresholds Section**
  - CPU Threshold slider (50-100%)
  - Memory Threshold slider (50-100%)
  - Disk Usage Threshold slider (50-100%)

- **Notifications Section**
  - Enable Notifications checkbox
  - Change Tray Icon Color on Alerts checkbox

- **Startup Behavior Section**
  - Start Minimized checkbox
  - Minimize to Tray checkbox

### 4. Integration Points

#### ApplicationCore
- Now accepts `SettingsService` parameter (optional)
- Loads settings on startup
- Uses settings to configure:
  - Refresh interval for metric collection
  - History size for metric cache
  - Alert thresholds and notification preferences

#### MainViewModel
- Creates `ApplicationCore` with settings support
- Exposes `GetCurrentSettings()` and `UpdateSettings()` methods
- Uses history size from settings for chart data points

#### App.xaml.cs
- Reads startup settings on application launch
- Implements start minimized behavior
- Implements minimize to tray behavior

#### User Interface
- Settings button added to main window header
- Settings menu item added to tray icon context menu
- Both open the settings dialog

## File Structure
```
src/
├── SystemHealthDashboard.Core/
│   ├── Models/
│   │   ├── ApplicationSettings.cs      (NEW)
│   │   └── AlertConfiguration.cs       (existing)
│   └── Services/
│       ├── SettingsService.cs          (NEW)
│       └── ApplicationCore.cs          (MODIFIED)
└── SystemHealthDashboard.UI/
    ├── SettingsWindow.xaml             (NEW)
    ├── SettingsWindow.xaml.cs          (NEW)
    ├── MainWindow.xaml                 (MODIFIED - added settings button)
    ├── MainWindow.xaml.cs              (MODIFIED - added click handler)
    ├── App.xaml                        (MODIFIED - added settings menu)
    ├── App.xaml.cs                     (MODIFIED - startup behavior)
    └── ViewModels/
        └── MainViewModel.cs            (MODIFIED - settings integration)
```

## Usage

### Accessing Settings
1. **From Main Window**: Click the "⚙️ Settings" button in the top-right corner
2. **From Tray Icon**: Right-click the tray icon and select "Settings"

### Modifying Settings
1. Open the Settings window
2. Adjust sliders or checkboxes as desired
3. Click "Save" to persist changes
4. Some settings (like refresh interval) require application restart

### Settings File Location
Settings are stored at: `%AppData%\SystemHealthDashboard\settings.json`

Example settings file:
```json
{
  "RefreshIntervalMs": 1000,
  "HistorySize": 60,
  "Thresholds": {
    "CpuThresholdPercent": 85.0,
    "CpuThresholdDurationSeconds": 10,
    "MemoryThresholdPercent": 80.0,
    "MemoryThresholdDurationSeconds": 10,
    "DiskUsageThresholdPercent": 90.0,
    "NotificationsEnabled": true,
    "TrayIconColorChangeEnabled": true
  },
  "Theme": {
    "Theme": "Dark",
    "AccentColor": "#FF6495ED"
  },
  "Startup": {
    "StartMinimized": false,
    "StartWithWindows": false,
    "MinimizeToTray": true
  }
}
```

## Technical Details

### Default Values
All settings have sensible defaults that are used when:
- Settings file doesn't exist
- Settings file is corrupted
- Specific settings are missing from the file

### Error Handling
- File read/write errors are caught and logged to console
- Application continues with default settings if persistence fails
- Invalid settings are replaced with defaults

### Restart Requirements
Changes to these settings require application restart:
- Refresh Interval
- History Size

Changes to these settings take effect immediately:
- All threshold values
- Notification preferences
- Startup behavior (applies on next launch)

## Future Enhancements
- Start with Windows registry integration
- Theme customization (Light/Dark/Custom)
- Custom accent colors
- Export/Import settings
- Reset to defaults button
- Settings validation and bounds checking
