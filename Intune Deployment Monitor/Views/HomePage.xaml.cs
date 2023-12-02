using Intune_Deployment_Monitor.ViewModels;
using Microsoft.UI.Xaml;
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

        Loaded += HomePage_Loaded;
    }

    private async void HomePage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.CheckForUpdates(this);
    }

    private async void ButtonAzure_Click(object sender, RoutedEventArgs e)
    {
        var uri = new Uri("https://www.portal.azure.com");

        // Launch the web URL
        await Windows.System.Launcher.LaunchUriAsync(uri);
    }

    private async void ButtonIntune_Click(object sender, RoutedEventArgs e)
    {
        var uri = new Uri("https://intune.microsoft.com/");

        // Launch the web URL
        await Windows.System.Launcher.LaunchUriAsync(uri);
    }

    private async void ButtonGithub_Click(object sender, RoutedEventArgs e)
    {
        var uri = new Uri("https://github.com/mchave3/Intune-Deployment-Monitor");

        // Launch the web URL
        await Windows.System.Launcher.LaunchUriAsync(uri);
    }

}