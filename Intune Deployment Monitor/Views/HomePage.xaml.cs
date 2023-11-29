using Intune_Deployment_Monitor.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Intune_Deployment_Monitor.Views;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel
    {
        get;
    }

    public HomePage()
    {
        ViewModel = App.GetService<HomeViewModel>();
        InitializeComponent();

        // Assuming you want to check for updates when the HomePage is loaded
        Loaded += HomePage_Loaded;
    }

    private async void HomePage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Pass 'this' as the UI element to the CheckForUpdates method
        await ViewModel.CheckForUpdates(this);
    }
}