using System.Windows;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using SystemHealthDashboard.UI.ViewModels;
using SystemHealthDashboard.UI.Services;

namespace SystemHealthDashboard.UI;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = (MainViewModel)DataContext;
    }

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);
        
        if (WindowState == WindowState.Minimized)
        {
            Hide();
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
        WindowState = WindowState.Minimized;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _viewModel?.Dispose();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow(_viewModel);
        settingsWindow.Owner = this;
        settingsWindow.ShowDialog();
    }

    private void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow();
        aboutWindow.Owner = this;
        aboutWindow.ShowDialog();
    }

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        var snapshot = SnapshotExporter.CreateSnapshot(
            _viewModel.GetCurrentCpuMetric(),
            _viewModel.GetCurrentMemoryMetric(),
            _viewModel.GetCurrentDiskMetric(),
            _viewModel.GetCurrentNetworkMetric(),
            _viewModel.GetCpuHistory(),
            _viewModel.GetMemoryHistory(),
            _viewModel.GetDiskHistory(),
            _viewModel.GetNetworkHistory()
        );

        var dialog = new SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt",
            DefaultExt = "json",
            FileName = $"system-snapshot-{DateTime.Now:yyyy-MM-dd-HHmmss}"
        };

        if (dialog.ShowDialog() == true)
        {
            string content = Path.GetExtension(dialog.FileName).ToLower() == ".json"
                ? SnapshotExporter.ExportToJson(snapshot)
                : SnapshotExporter.ExportToText(snapshot);

            SnapshotExporter.SaveToFile(content, dialog.FileName);
            MessageBox.Show($"Snapshot exported successfully to:\n{dialog.FileName}", 
                "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}