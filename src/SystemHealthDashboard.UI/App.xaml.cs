using System.Configuration;
using System.Data;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using SystemHealthDashboard.Core.Models;
using SystemHealthDashboard.UI.Helpers;
using SystemHealthDashboard.UI.ViewModels;

namespace SystemHealthDashboard.UI;

public partial class App : Application
{
    private TaskbarIcon? _trayIcon;
    private MainViewModel? _mainViewModel;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Show splash screen
        var splash = new SplashWindow();
        splash.Show();

        // Initialize application (simulate initialization time)
        Task.Run(async () =>
        {
            await Task.Delay(1000);
            splash.UpdateStatus("Loading metrics...");
            await Task.Delay(500);
            splash.UpdateStatus("Initializing dashboard...");
            await Task.Delay(500);
            splash.UpdateStatus("Ready!");
            await Task.Delay(300);

            Dispatcher.Invoke(() =>
            {
                splash.Close();
                InitializeMainWindow();
            });
        });
    }

    private void InitializeMainWindow()
    {
        _trayIcon = (TaskbarIcon)FindResource("TrayIcon");
        
        if (_trayIcon != null)
        {
            _trayIcon.Icon = TrayIconHelper.CreateTrayIcon(AlertSeverity.Normal);
            _trayIcon.TrayLeftMouseDown += (s, args) => ShowMainWindow();
        }

        if (MainWindow?.DataContext is MainViewModel viewModel)
        {
            _mainViewModel = viewModel;
            _mainViewModel.AlertSeverityChanged += OnAlertSeverityChanged;
            
            var settings = viewModel.GetCurrentSettings();
            if (settings.Startup.StartMinimized)
            {
                MainWindow.WindowState = WindowState.Minimized;
                if (settings.Startup.MinimizeToTray)
                {
                    MainWindow.Hide();
                }
            }
        }
    }

    private void OnAlertSeverityChanged(object? sender, AlertSeverity severity)
    {
        Dispatcher.Invoke(() =>
        {
            if (_trayIcon != null)
            {
                _trayIcon.Icon = TrayIconHelper.CreateTrayIcon(severity);
            }
        });
    }

    private void ShowMainWindow()
    {
        if (MainWindow != null)
        {
            MainWindow.Show();
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate();
        }
    }

    private void ShowDashboard_Click(object sender, RoutedEventArgs e)
    {
        ShowMainWindow();
    }

    private void ShowSettings_Click(object sender, RoutedEventArgs e)
    {
        if (_mainViewModel != null)
        {
            var settingsWindow = new SettingsWindow(_mainViewModel)
            {
                Owner = MainWindow
            };
            settingsWindow.ShowDialog();
        }
    }

    private void ShowAbout_Click(object sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow
        {
            Owner = MainWindow
        };
        aboutWindow.ShowDialog();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        _trayIcon?.Dispose();
        Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayIcon?.Dispose();
        base.OnExit(e);
    }
}

