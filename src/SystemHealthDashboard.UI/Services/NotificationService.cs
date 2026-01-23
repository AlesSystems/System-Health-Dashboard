using System.Windows;
using SystemHealthDashboard.Core.Models;

namespace SystemHealthDashboard.UI.Services;

public class NotificationService
{
    private readonly AlertConfiguration _config;

    public NotificationService(AlertConfiguration config)
    {
        _config = config;
    }

    public void ShowAlert(Alert alert)
    {
        if (!_config.NotificationsEnabled)
            return;

        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(
                alert.Message,
                $"System Health Alert - {alert.Type}",
                MessageBoxButton.OK,
                alert.Severity == AlertSeverity.Critical ? MessageBoxImage.Error : MessageBoxImage.Warning
            );
        });
    }

    public void ShowTrayNotification(string title, string message, Hardcodet.Wpf.TaskbarNotification.TaskbarIcon? trayIcon)
    {
        if (!_config.NotificationsEnabled || trayIcon == null)
            return;

        Application.Current.Dispatcher.Invoke(() =>
        {
            trayIcon.ShowBalloonTip(title, message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
        });
    }
}
