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

        // Attach event handlers to the button click events
        ButtonConnect.Click += ButtonConnect_Click;
        ButtonDisconnect.Click += ButtonDisconnect_Click;

        // Attempt silent login on startup
        SilentLogin();
    }

    private async void SilentLogin()
    {
        try
        {
            bool isLoggedIn = await AuthMicrosoftService.SilentLogin();
            // Update UI based on the login status
            UpdateUI(isLoggedIn);
        }
        catch
        {
            // If silent login fails, update UI to show the connect button
            UpdateUI(false);
        }
    }

    private void UpdateUI(bool isLoggedIn)
    {
        // Adjust visibility of Connect and Disconnect buttons based on login status
        ButtonConnect.Visibility = isLoggedIn ? Visibility.Collapsed : Visibility.Visible;
        ButtonDisconnect.Visibility = isLoggedIn ? Visibility.Visible : Visibility.Collapsed;
    }

    // Event handler for the Connect button
    private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await AuthMicrosoftService.Login();
            // Update UI to reflect the user is logged in
            UpdateUI(true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during login: {ex.ToString()}");
        }
    }

    // Event handler for the Disconnect button
    private async void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await AuthMicrosoftService.Logout();
            // Update UI to reflect the user is logged out
            UpdateUI(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during logout: {ex.Message}");
        }
    }
}
