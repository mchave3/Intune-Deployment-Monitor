using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using Intune_Deployment_Monitor.Services;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Intune_Deployment_Monitor.ViewModels
{
    // ViewModel for the Home page that handles user login state and UI updates
    public partial class HomeViewModel : ObservableRecipient
    {
        // Services used by the ViewModel
        private readonly UpdateViewModel _updateViewModel;
        private readonly MicrosoftGraphService _microsoftGraphService;

        // Fields to keep track of UI element visibility
        private Visibility _connectButtonVisibility = Visibility.Visible;
        private Visibility _disconnectButtonVisibility = Visibility.Collapsed;
        private bool _isLoggedIn;

        // Fields to display user information
        private string _displayName;
        private string _versionNumber;

        // Constructor initializing commands and services
        public HomeViewModel()
        {
            _updateViewModel = new UpdateViewModel();
            _microsoftGraphService = new MicrosoftGraphService();

            ConnectCommand = new RelayCommand(ExecuteConnect);
            DisconnectCommand = new RelayCommand(ExecuteDisconnect);

            VersionNumber = GetVersionNumber();
            SilentLogin();
        }

        // ICommand properties for binding buttons to actions
        public ICommand ConnectCommand
        {
            get;
        }
        public ICommand DisconnectCommand
        {
            get;
        }

        // Visibility properties for Connect and Disconnect buttons
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

        // Property to hold the application's version number
        public string VersionNumber
        {
            get => _versionNumber;
            set => SetProperty(ref _versionNumber, value);
        }

        // Property to control the visibility of the welcome message
        public Visibility WelcomeMessageVisibility => IsLoggedIn ? Visibility.Visible : Visibility.Collapsed;

        // Property to control the visibility of the login prompt
        public Visibility LoginPromptVisibility => IsLoggedIn ? Visibility.Collapsed : Visibility.Visible;

        // Property to track the login status
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            private set
            {
                SetProperty(ref _isLoggedIn, value);
                OnPropertyChanged(nameof(LoginPromptVisibility));
                OnPropertyChanged(nameof(WelcomeMessageVisibility));
            }
        }

        // Property to display the user's display name
        public string DisplayName
        {
            get => _displayName;
            set
            {
                SetProperty(ref _displayName, value);
                OnPropertyChanged(nameof(WelcomeMessageVisibility));
            }
        }

        // Retrieves the version number of the application
        private string GetVersionNumber()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        // Updates the UI based on the user's login status
        private void UpdateUI(bool isLoggedIn)
        {
            IsLoggedIn = isLoggedIn;
            ConnectButtonVisibility = isLoggedIn ? Visibility.Collapsed : Visibility.Visible;
            DisconnectButtonVisibility = isLoggedIn ? Visibility.Visible : Visibility.Collapsed;
        }

        // Command execution methods for login and logout
        private async void ExecuteConnect()
        {
            await ExecuteConnectAsync();
        }

        private async Task ExecuteConnectAsync()
        {
            try
            {
                bool loginSuccessful = await AuthMicrosoftService.Login();
                if (loginSuccessful)
                {
                    DisplayName = await _microsoftGraphService.GetUserDisplayNameAsync();
                    UpdateUI(true);
                }
                else
                {
                    DisplayName = string.Empty;
                    UpdateUI(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during login: {ex}");
                UpdateUI(false);
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

        // Attempts a silent login without user interaction
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

        // Checks for application updates
        public async Task CheckForUpdates(UIElement uiElement)
        {
            await _updateViewModel.CheckForUpdates(uiElement);
        }
    }
}
