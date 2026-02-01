using System.Windows;
using System.Windows.Media;

namespace SystemHealthDashboard.UI.Helpers;

public enum ModernMessageBoxType
{
    Information,
    Success,
    Warning,
    Error
}

public partial class ModernMessageBox : Window
{
    public ModernMessageBox(string title, string message, ModernMessageBoxType type = ModernMessageBoxType.Information, bool showCancelButton = false)
    {
        InitializeComponent();
        
        TitleText.Text = title;
        MessageText.Text = message;
        
        ConfigureMessageType(type);
        ConfigureButtons(type, showCancelButton);
    }

    private void ConfigureMessageType(ModernMessageBoxType type)
    {
        switch (type)
        {
            case ModernMessageBoxType.Information:
                IconText.Text = "ℹ️";
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(100, 149, 237)); // CornflowerBlue
                break;
            
            case ModernMessageBoxType.Success:
                IconText.Text = "✅";
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(60, 179, 113)); // MediumSeaGreen
                break;
            
            case ModernMessageBoxType.Warning:
                IconText.Text = "⚠️";
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0)); // Orange
                break;
            
            case ModernMessageBoxType.Error:
                IconText.Text = "❌";
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(220, 20, 60)); // Crimson
                break;
        }
    }

    private void ConfigureButtons(ModernMessageBoxType type, bool showCancelButton)
    {
        if (type == ModernMessageBoxType.Warning)
        {
            YesButton.Content = "Yes";
            NoButton.Content = "No";
            NoButton.Visibility = Visibility.Visible;
            
            if (showCancelButton)
            {
                CancelButton.Visibility = Visibility.Visible;
            }
        }
        else
        {
            YesButton.Content = "OK";
            NoButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
        }
    }

    private void YesButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void NoButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = null;
        Close();
    }
}
