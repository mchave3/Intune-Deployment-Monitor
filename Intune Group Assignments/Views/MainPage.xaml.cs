using System.Diagnostics;
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
    private void ButtonConnect_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Debug.WriteLine("Toto");
            // Create an instance of AuthMicrosoftService
            // AuthMicrosoftService authMicrosoftService = new AuthMicrosoftService();

            // Call the AuthMicrosoft method on the instance
            // await AuthMicrosoftService.AuthMicrosoft();
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
            // Call the method on the existing instance
            // await _authMicrosoftService.LogoutAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.ToString()}");
        }
    }
}
