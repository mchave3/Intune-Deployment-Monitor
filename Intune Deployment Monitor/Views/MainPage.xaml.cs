using Intune_Deployment_Monitor.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Intune_Deployment_Monitor.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();

        // Assuming you want to check for updates when the MainPage is loaded
        Loaded += MainPage_Loaded;
    }

    private async void MainPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Pass 'this' as the UI element to the CheckForUpdates method
        await ViewModel.CheckForUpdates(this);
    }
}