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

