using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using Intune_Group_Assignments.Services;
using Microsoft.UI.Xaml;
using System.Diagnostics;

namespace Intune_Group_Assignments.ViewModels
{
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
            try
            {
                await AuthMicrosoftService.Login();
                DisplayName = await _microsoftGraphService.GetUserDisplayNameAsync();
                UpdateUI(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during login: {ex}");
                UpdateUI(false);
            }
        }

        private async void ExecuteDisconnect()
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
    }
}
