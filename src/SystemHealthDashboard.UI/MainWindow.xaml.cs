using System.Windows;
using SystemHealthDashboard.UI.ViewModels;

namespace SystemHealthDashboard.UI;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = (MainViewModel)DataContext;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _viewModel?.Dispose();
    }
}