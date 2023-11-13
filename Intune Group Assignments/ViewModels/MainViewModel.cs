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
    private Visibility _connectButtonVisibility = Visibility.Visible;
    private Visibility _disconnectButtonVisibility = Visibility.Collapsed;

    public MainViewModel()
    {
        ConnectCommand = new RelayCommand(ExecuteConnect);
        DisconnectCommand = new RelayCommand(ExecuteDisconnect);
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

    private async void ExecuteConnect()
    {
        try
        {
            await AuthMicrosoftService.Login();
            UpdateUI(true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during login: {ex}");
            UpdateUI(false); // Update UI in case of an exception
        }
    }

    private async void ExecuteDisconnect()
    {
        try
        {
            await AuthMicrosoftService.Logout();
            UpdateUI(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during logout: {ex}");
            UpdateUI(true); // Update UI in case of an exception
        }
    }

    private async void SilentLogin()
    {
        try
        {
            bool isLoggedIn = await AuthMicrosoftService.SilentLogin();
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
}
