using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using Intune_Group_Assignments.Services;
using Microsoft.UI.Xaml;
using System.Diagnostics;

namespace Intune_Group_Assignments.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    private readonly MicrosoftGraphService _microsoftGraphService;
    private Visibility _connectButtonVisibility = Visibility.Visible;
    private Visibility _disconnectButtonVisibility = Visibility.Collapsed;
    private string _displayName;

    public MainViewModel()
    {
        _microsoftGraphService = new MicrosoftGraphService();

        ConnectCommand = new RelayCommand(ExecuteConnect);
        DisconnectCommand = new RelayCommand(ExecuteDisconnect);
        TestCommand = new RelayCommand(ExecuteTest);
        SilentLogin();
    }

    public ICommand ConnectCommand
    {
        get;
    }
    public ICommand DisconnectCommand
    {
        get;
    }

    public ICommand TestCommand
    {
    
           get;
    }

    public Visibility ConnectButtonVisibility
    {
        get => _connectButtonVisibility;
        set => SetProperty(ref _connectButtonVisibility, value);
    }

    public Visibility DisconnectButtonVisibility
    {
        get => _disconnectButtonVisibility;
        set => SetProperty(ref _disconnectButtonVisibility, value);
    }

    public Visibility WelcomeMessageVisibility { get; private set; } = Visibility.Collapsed;

    public string DisplayName
    {
        get => _displayName;
        set
        {
            SetProperty(ref _displayName, value);
            UpdateWelcomeMessageVisibility();
        }
    }

    private void UpdateWelcomeMessageVisibility()
    {
        WelcomeMessageVisibility = string.IsNullOrEmpty(DisplayName) ? Visibility.Collapsed : Visibility.Visible;
        OnPropertyChanged(nameof(WelcomeMessageVisibility));
    }

    private async void ExecuteConnect()
    {
        await ExecuteConnectAsync();
    }

    private async Task ExecuteConnectAsync()
    {
        try
        {
            // Attempt to login and check if it was successful
            bool loginSuccessful = await AuthMicrosoftService.Login();
            if (loginSuccessful)
            {
                // If login is successful, retrieve the user's display name and update the UI
                DisplayName = await _microsoftGraphService.GetUserDisplayNameAsync();
                UpdateUI(true); // isLoggedIn is set to true
            }
            else
            {
                // If login was not successful, ensure that the UI is set to a logged out state
                DisplayName = string.Empty;
                UpdateUI(false); // isLoggedIn is set to false
            }
        }
        catch (Exception ex)
        {
            // If an exception is caught, log it and set the UI to a logged out state
            Debug.WriteLine($"Error during login: {ex}");
            UpdateUI(false); // isLoggedIn is set to false
        }
    }

    private async void ExecuteDisconnect()
    {
        await ExecuteDisconnectAsync();
    }

    private async Task ExecuteDisconnectAsync()
    {
        try
        {
            await AuthMicrosoftService.Logout();
            DisplayName = string.Empty;
            UpdateUI(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during logout: {ex}");
            UpdateUI(true);
        }
    }

    private async void SilentLogin()
    {
        try
        {
            bool isLoggedIn = await AuthMicrosoftService.SilentLogin();
            if (isLoggedIn)
            {
                DisplayName = await _microsoftGraphService.GetUserDisplayNameAsync();
            }
            UpdateUI(isLoggedIn);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during silent login: {ex}");
            UpdateUI(false);
        }
    }

    private void UpdateUI(bool isLoggedIn)
    {
        ConnectButtonVisibility = isLoggedIn ? Visibility.Collapsed : Visibility.Visible;
        DisconnectButtonVisibility = isLoggedIn ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void ExecuteTest()
    {
        DisplayName = await _microsoftGraphService.GetUserDisplayNameAsync();
    }
}
