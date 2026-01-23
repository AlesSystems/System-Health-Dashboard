using System.Windows;

namespace SystemHealthDashboard.UI;

public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        InitializeComponent();
    }

    public void UpdateStatus(string message)
    {
        Dispatcher.Invoke(() =>
        {
            LoadingText.Text = message;
        });
    }
}
