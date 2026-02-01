using System.Windows;
using System.Windows.Media;
using SystemHealthDashboard.Core.Models;
using SystemHealthDashboard.UI.ViewModels;
using SystemHealthDashboard.UI.Helpers;

namespace SystemHealthDashboard.UI;

public partial class SettingsWindow : Window
{
    private readonly MainViewModel _mainViewModel;
    private ApplicationSettings _settings = new();
    private ApplicationSettings _originalSettings = new();
    private bool _isLoading = false;
    private bool _hasUnsavedChanges = false;

    public SettingsWindow(MainViewModel viewModel)
    {
        try
        {
            InitializeComponent();
            _mainViewModel = viewModel;
            
            // Create a copy of settings to avoid modifying the original until save
            var currentSettings = _mainViewModel.GetCurrentSettings();
            _originalSettings = CloneSettings(currentSettings);
            _settings = CloneSettings(currentSettings);
            
            Loaded += SettingsWindow_Loaded;
            Closing += SettingsWindow_Closing;
        }
        catch (Exception ex)
        {
            ShowErrorPopup("Initialization Error", $"Failed to initialize settings window: {ex.Message}");
            throw;
        }
    }

    private ApplicationSettings CloneSettings(ApplicationSettings source)
    {
        return new ApplicationSettings
        {
            RefreshIntervalMs = source.RefreshIntervalMs,
            HistorySize = source.HistorySize,
            Thresholds = new ThresholdSettings
            {
                CpuThresholdPercent = source.Thresholds.CpuThresholdPercent,
                CpuThresholdDurationSeconds = source.Thresholds.CpuThresholdDurationSeconds,
                MemoryThresholdPercent = source.Thresholds.MemoryThresholdPercent,
                MemoryThresholdDurationSeconds = source.Thresholds.MemoryThresholdDurationSeconds,
                DiskUsageThresholdPercent = source.Thresholds.DiskUsageThresholdPercent,
                NotificationsEnabled = source.Thresholds.NotificationsEnabled,
                TrayIconColorChangeEnabled = source.Thresholds.TrayIconColorChangeEnabled
            },
            Theme = new ThemeSettings
            {
                Theme = source.Theme.Theme,
                AccentColor = source.Theme.AccentColor
            },
            Startup = new StartupSettings
            {
                StartMinimized = source.Startup.StartMinimized,
                StartWithWindows = source.Startup.StartWithWindows,
                MinimizeToTray = source.Startup.MinimizeToTray
            }
        };
    }

    private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _isLoading = true;
            LoadSettings();
            _isLoading = false;
            _hasUnsavedChanges = false;
        }
        catch (Exception ex)
        {
            ShowErrorPopup("Load Error", $"Failed to load settings: {ex.Message}");
        }
    }

    private void SettingsWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_hasUnsavedChanges)
        {
            var result = ShowWarningPopup(
                "Unsaved Changes",
                "You have unsaved changes. Do you want to save them before closing?",
                showCancelButton: true);
            
            if (result == true)
            {
                SaveButton_Click(this, new RoutedEventArgs());
            }
            else if (result == null)
            {
                e.Cancel = true;
            }
        }
    }

    private void LoadSettings()
    {
        RefreshIntervalSlider.Value = _settings.RefreshIntervalMs;
        HistorySizeSlider.Value = _settings.HistorySize;
        CpuThresholdSlider.Value = _settings.Thresholds.CpuThresholdPercent;
        MemoryThresholdSlider.Value = _settings.Thresholds.MemoryThresholdPercent;
        DiskThresholdSlider.Value = _settings.Thresholds.DiskUsageThresholdPercent;
        NotificationsCheckBox.IsChecked = _settings.Thresholds.NotificationsEnabled;
        TrayIconCheckBox.IsChecked = _settings.Thresholds.TrayIconColorChangeEnabled;
        StartMinimizedCheckBox.IsChecked = _settings.Startup.StartMinimized;
        MinimizeToTrayCheckBox.IsChecked = _settings.Startup.MinimizeToTray;
    }

    private void MarkAsChanged()
    {
        if (!_isLoading)
        {
            _hasUnsavedChanges = true;
            UnsavedChangesIndicator.Visibility = Visibility.Visible;
        }
    }

    private void RefreshIntervalSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (RefreshIntervalText != null && _settings != null)
        {
            var value = (int)e.NewValue;
            RefreshIntervalText.Text = $"{value} ms";
            _settings.RefreshIntervalMs = value;
            MarkAsChanged();
        }
    }

    private void HistorySizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (HistorySizeText != null && _settings != null)
        {
            var value = (int)e.NewValue;
            HistorySizeText.Text = $"{value} points";
            _settings.HistorySize = value;
            MarkAsChanged();
        }
    }

    private void CpuThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (CpuThresholdText != null && _settings != null)
        {
            var value = e.NewValue;
            CpuThresholdText.Text = $"{value:F0}%";
            _settings.Thresholds.CpuThresholdPercent = value;
            MarkAsChanged();
        }
    }

    private void MemoryThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (MemoryThresholdText != null && _settings != null)
        {
            var value = e.NewValue;
            MemoryThresholdText.Text = $"{value:F0}%";
            _settings.Thresholds.MemoryThresholdPercent = value;
            MarkAsChanged();
        }
    }

    private void DiskThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (DiskThresholdText != null && _settings != null)
        {
            var value = e.NewValue;
            DiskThresholdText.Text = $"{value:F0}%";
            _settings.Thresholds.DiskUsageThresholdPercent = value;
            MarkAsChanged();
        }
    }

    private void CheckBox_Changed(object sender, RoutedEventArgs e)
    {
        MarkAsChanged();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Update checkbox values
            _settings.Thresholds.NotificationsEnabled = NotificationsCheckBox.IsChecked ?? true;
            _settings.Thresholds.TrayIconColorChangeEnabled = TrayIconCheckBox.IsChecked ?? true;
            _settings.Startup.StartMinimized = StartMinimizedCheckBox.IsChecked ?? false;
            _settings.Startup.MinimizeToTray = MinimizeToTrayCheckBox.IsChecked ?? true;
            
            // Validate settings
            if (!ValidateSettings())
            {
                return;
            }
            
            _mainViewModel.UpdateSettings(_settings);
            _originalSettings = CloneSettings(_settings);
            _hasUnsavedChanges = false;
            UnsavedChangesIndicator.Visibility = Visibility.Collapsed;
            
            ShowSuccessPopup(
                "Settings Saved",
                "Settings have been saved successfully!\n\nNote: Some settings like refresh interval and history size may require an application restart to take full effect.");
            
            Close();
        }
        catch (Exception ex)
        {
            ShowErrorPopup("Save Error", $"Failed to save settings: {ex.Message}");
        }
    }

    private bool ValidateSettings()
    {
        if (_settings.RefreshIntervalMs < 100 || _settings.RefreshIntervalMs > 5000)
        {
            ShowWarningPopup("Validation Error", "Refresh interval must be between 100 and 5000 milliseconds.");
            return false;
        }

        if (_settings.HistorySize < 30 || _settings.HistorySize > 300)
        {
            ShowWarningPopup("Validation Error", "History size must be between 30 and 300 points.");
            return false;
        }

        if (_settings.Thresholds.CpuThresholdPercent < 50 || _settings.Thresholds.CpuThresholdPercent > 100)
        {
            ShowWarningPopup("Validation Error", "CPU threshold must be between 50% and 100%.");
            return false;
        }

        if (_settings.Thresholds.MemoryThresholdPercent < 50 || _settings.Thresholds.MemoryThresholdPercent > 100)
        {
            ShowWarningPopup("Validation Error", "Memory threshold must be between 50% and 100%.");
            return false;
        }

        if (_settings.Thresholds.DiskUsageThresholdPercent < 50 || _settings.Thresholds.DiskUsageThresholdPercent > 100)
        {
            ShowWarningPopup("Validation Error", "Disk threshold must be between 50% and 100%.");
            return false;
        }

        return true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        var result = ShowWarningPopup(
            "Reset to Defaults",
            "Are you sure you want to reset all settings to their default values?",
            showCancelButton: true);
        
        if (result == true)
        {
            _settings = new ApplicationSettings();
            _isLoading = true;
            LoadSettings();
            _isLoading = false;
            _hasUnsavedChanges = true;
        }
    }

    private void ShowSuccessPopup(string title, string message)
    {
        var popup = new ModernMessageBox(title, message, ModernMessageBoxType.Success);
        popup.Owner = this;
        popup.ShowDialog();
    }

    private void ShowErrorPopup(string title, string message)
    {
        var popup = new ModernMessageBox(title, message, ModernMessageBoxType.Error);
        popup.Owner = this;
        popup.ShowDialog();
    }

    private bool? ShowWarningPopup(string title, string message, bool showCancelButton = false)
    {
        var popup = new ModernMessageBox(title, message, ModernMessageBoxType.Warning, showCancelButton);
        popup.Owner = this;
        return popup.ShowDialog();
    }
}
