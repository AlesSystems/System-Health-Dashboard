# Manual Testing Guide

This document provides instructions for manually testing the System Health Dashboard application.

## Phase 10: Testing Strategy

### Automated Tests

Run the automated test suite:
```bash
dotnet test
```

The test suite includes:
- **Metric Parsing Tests**: Validates correct calculation of CPU, Memory, Disk, and Network metrics
- **Scheduler Timing Tests**: Ensures metrics are collected at proper intervals and history is maintained

### Manual Tests

#### 1. Stress CPU/RAM Test

**CPU Stress:**
1. Launch the application
2. Open Task Manager (Ctrl+Shift+Esc)
3. Open a browser and run CPU-intensive tasks (multiple tabs with heavy websites, video playback)
4. Observe the CPU metric in the dashboard should rise above 50%
5. Verify the CPU chart updates in real-time
6. Check if alerts are triggered when CPU exceeds configured threshold (default 80%)

**RAM Stress:**
1. Open multiple applications (browsers, editors, etc.)
2. Open many browser tabs
3. Monitor memory usage rising in the dashboard
4. Verify the memory percentage and GB values are accurate
5. Check alerts when memory exceeds threshold (default 85%)

#### 2. Network Stress Test

**Steps:**
1. Start a large file download
2. Upload files to cloud storage
3. Stream video content
4. Observe network download/upload metrics updating
5. Verify the network traffic chart reflects the activity

#### 3. Disk I/O Test

**Steps:**
1. Copy large files between drives
2. Run a file indexing/search operation
3. Install or update software
4. Monitor disk read/write metrics
5. Verify disk I/O chart shows activity spikes

#### 4. Edge Cases

**Sleep/Resume Test:**
1. Put computer to sleep for 1-2 minutes
2. Resume the computer
3. Verify the application continues collecting metrics
4. Check that the dashboard updates correctly
5. Ensure no crashes or frozen UI

**Laptop Battery Mode:**
1. If using a laptop, disconnect power
2. Switch to battery mode
3. Verify metrics continue to update
4. Test performance throttling detection
5. Reconnect power and verify recovery

**VM Environment:**
1. If running in a VM, verify all metrics are collected
2. Check CPU core detection is accurate
3. Verify network interface detection
4. Test memory calculations

### Settings Tests

1. **Refresh Interval:**
   - Change from 1000ms to 2000ms
   - Verify charts update slower
   - Change back to 1000ms

2. **Alert Thresholds:**
   - Lower CPU threshold to 20%
   - Trigger an alert by running CPU-intensive task
   - Verify notification appears
   - Verify tray icon changes color (if enabled)

3. **Startup Options:**
   - Enable "Start with Windows"
   - Enable "Start Minimized"
   - Enable "Minimize to Tray"
   - Restart application and verify settings are applied

## Phase 11: Polishing Features

### Splash Screen Test

1. Close the application completely
2. Restart the application
3. Verify splash screen appears with:
   - Application icon/logo
   - Loading progress messages
   - Smooth transition to main window

### About Window Test

1. Click the "About" button in the main window
2. Verify version information is displayed correctly
3. Check company and copyright information
4. Close the About window

### Export Snapshot Test

**JSON Export:**
1. Click "Export" button
2. Select "JSON files (*.json)"
3. Choose a save location
4. Open the exported file and verify:
   - System information is present
   - Current metrics are included
   - History data is included
   - JSON is properly formatted

**Text Export:**
1. Click "Export" button
2. Select "Text files (*.txt)"
3. Choose a save location
4. Open the exported file and verify:
   - Readable format
   - All metrics are present
   - System information is included

### Tray Icon Features

1. Minimize to tray and verify:
   - Icon appears in system tray
   - Right-click shows context menu
   - "Show Dashboard" opens the window
   - "Settings" opens settings dialog
   - "About" opens about window
   - "Exit" closes the application

2. Alert severity test:
   - Trigger a critical alert (CPU > 90%)
   - Verify tray icon changes color (if enabled)
   - Clear the alert condition
   - Verify tray icon returns to normal

### UI Responsiveness

1. Resize the main window
2. Minimize and restore
3. Move between monitors (if available)
4. Verify no UI glitches or freezing
5. Check charts remain smooth during updates

## Success Criteria

All tests should pass with:
- ✅ No crashes or exceptions
- ✅ Accurate metric reporting (±5% tolerance)
- ✅ Smooth UI updates (no flickering)
- ✅ Correct alert triggering
- ✅ Proper settings persistence
- ✅ All polishing features working
- ✅ Responsive user interface
