using System.Diagnostics;
using Intune_Group_Assignments.Services;
using Intune_Group_Assignments.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Intune_Group_Assignments.Views;

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

        // Attach the event handler to the button click event
        ButtonConnect.Click += ButtonConnect_Click;
        ButtonDisconnect.Click += ButtonDisconnect_Click;
    }

    // Event handler for connection button
    private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await AuthMicrosoftService.Login();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.ToString()}");
        }
    }

    // Event handler for disconnect button
    private async void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await AuthMicrosoftService.Logout();
            // Optionally, add any UI updates or notifications here to indicate successful logout
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during logout: {ex.Message}");
        }
    }
}
