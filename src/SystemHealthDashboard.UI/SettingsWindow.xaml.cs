using System.Windows;
using SystemHealthDashboard.Core.Models;
using SystemHealthDashboard.UI.ViewModels;

namespace SystemHealthDashboard.UI;

public partial class SettingsWindow : Window
{
    private readonly MainViewModel _mainViewModel;
    private ApplicationSettings _settings = new();

    public SettingsWindow(MainViewModel viewModel)
    {
        try
        {
            InitializeComponent();
            _mainViewModel = viewModel;
            
            // Create a copy of settings to avoid modifying the original until save
            var currentSettings = _mainViewModel.GetCurrentSettings();
            _settings = new ApplicationSettings
            {
                RefreshIntervalMs = currentSettings.RefreshIntervalMs,
                HistorySize = currentSettings.HistorySize,
                Thresholds = new ThresholdSettings
                {
                    CpuThresholdPercent = currentSettings.Thresholds.CpuThresholdPercent,
                    CpuThresholdDurationSeconds = currentSettings.Thresholds.CpuThresholdDurationSeconds,
                    MemoryThresholdPercent = currentSettings.Thresholds.MemoryThresholdPercent,
                    MemoryThresholdDurationSeconds = currentSettings.Thresholds.MemoryThresholdDurationSeconds,
                    DiskUsageThresholdPercent = currentSettings.Thresholds.DiskUsageThresholdPercent,
                    NotificationsEnabled = currentSettings.Thresholds.NotificationsEnabled,
                    TrayIconColorChangeEnabled = currentSettings.Thresholds.TrayIconColorChangeEnabled
                },
                Theme = new ThemeSettings
                {
                    Theme = currentSettings.Theme.Theme,
                    AccentColor = currentSettings.Theme.AccentColor
                },
                Startup = new StartupSettings
                {
                    StartMinimized = currentSettings.Startup.StartMinimized,
                    StartWithWindows = currentSettings.Startup.StartWithWindows,
                    MinimizeToTray = currentSettings.Startup.MinimizeToTray
                }
            };
            
            Loaded += SettingsWindow_Loaded;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing settings window: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }
    }

    private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            LoadSettings();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading settings: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

    private void RefreshIntervalSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (RefreshIntervalText != null && _settings != null)
        {
            var value = (int)e.NewValue;
            RefreshIntervalText.Text = $"{value} ms";
            _settings.RefreshIntervalMs = value;
        }
    }

    private void HistorySizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (HistorySizeText != null && _settings != null)
        {
            var value = (int)e.NewValue;
            HistorySizeText.Text = $"{value} points";
            _settings.HistorySize = value;
        }
    }

    private void CpuThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (CpuThresholdText != null && _settings != null)
        {
            var value = e.NewValue;
            CpuThresholdText.Text = $"{value:F0}%";
            _settings.Thresholds.CpuThresholdPercent = value;
        }
    }

    private void MemoryThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (MemoryThresholdText != null && _settings != null)
        {
            var value = e.NewValue;
            MemoryThresholdText.Text = $"{value:F0}%";
            _settings.Thresholds.MemoryThresholdPercent = value;
        }
    }

    private void DiskThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (DiskThresholdText != null && _settings != null)
        {
            var value = e.NewValue;
            DiskThresholdText.Text = $"{value:F0}%";
            _settings.Thresholds.DiskUsageThresholdPercent = value;
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _settings.Thresholds.NotificationsEnabled = NotificationsCheckBox.IsChecked ?? true;
            _settings.Thresholds.TrayIconColorChangeEnabled = TrayIconCheckBox.IsChecked ?? true;
            _settings.Startup.StartMinimized = StartMinimizedCheckBox.IsChecked ?? false;
            _settings.Startup.MinimizeToTray = MinimizeToTrayCheckBox.IsChecked ?? true;
            
            _mainViewModel.UpdateSettings(_settings);
            
            MessageBox.Show(
                "Settings saved successfully!\n\nNote: Some settings like refresh interval and history size will take effect after restarting the application.",
                "Settings Saved",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
