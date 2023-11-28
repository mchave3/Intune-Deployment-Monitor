using System.ComponentModel;
using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.Win32;

public class UpdateViewModel : INotifyPropertyChanged
{
    private readonly UpdateService _updateService;
    private string _latestVersion;

    public event PropertyChangedEventHandler PropertyChanged;

    public string LatestVersion
    {
        get => _latestVersion;
        set
        {
            _latestVersion = value;
            OnPropertyChanged(nameof(LatestVersion));
        }
    }

    public ICommand CheckForUpdatesCommand
    {
        get;
    }

    public UpdateViewModel()
    {
        _updateService = new UpdateService();
    }

    public async Task CheckForUpdates(UIElement uiElement)
    {
        try
        {
            var updateInfo = await _updateService.CheckForUpdatesAsync();
            var currentVersion = GetCurrentVersion();

            if (IsNewVersionAvailable(currentVersion, updateInfo.Version))
            {
                Debug.WriteLine("New version available");
                LatestVersion = updateInfo.Version;
                var dialog = new ContentDialog
                {
                    Title = "Update Available",
                    Content = $"A new version {updateInfo.Version} is available. Would you like to update now?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "Cancel"
                };

                if (uiElement.XamlRoot != null)
                {
                    dialog.XamlRoot = uiElement.XamlRoot;
                }
                Debug.WriteLine("Showing dialog");
                var result = await dialog.ShowAsync();
                Debug.WriteLine($"Dialog result: {result}");

                if (result == ContentDialogResult.Primary)
                {
                    await PerformUpdate(updateInfo.DownloadUrl);
                }
            }
            else
            {
                Debug.WriteLine("No new version available");
            }
        }
        catch (Exception ex)
        {
            // Log the exception for debugging purposes
            Debug.WriteLine($"An error occurred while checking for updates: {ex.Message}");

            // Inform the user that an error occurred
            var errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = "An error occurred while checking for updates. Please try again later.",
                CloseButtonText = "Ok"
            };

            if (uiElement.XamlRoot != null)
            {
                errorDialog.XamlRoot = uiElement.XamlRoot;
            }

            await errorDialog.ShowAsync();
            Debug.WriteLine("Error dialog shown");
        }
    }

    private bool IsNewVersionAvailable(string currentVersion, string latestVersion)
    {
        // Normalize the versions to have the same number of segments
        var currentVersionSegments = currentVersion.Split('.');
        var latestVersionSegments = latestVersion.Split('.');

        while (latestVersionSegments.Length < currentVersionSegments.Length)
        {
            latestVersion += ".0";
            latestVersionSegments = latestVersion.Split('.');
        }

        return !string.Equals(currentVersion, latestVersion, StringComparison.Ordinal);
    }

    private string GetCurrentVersion()
    {
        const string registryKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Mickael CHAVE\Intune Deployment Monitor";
        const string versionValueName = "Version";

        Debug.WriteLine($"Checking version in registry at: {registryKeyPath}");

        try
        {
            // Retrieve the value from the registry key
            string version = Registry.GetValue(registryKeyPath, versionValueName, null) as string;

            if (version != null)
            {
                Debug.WriteLine($"Version found in registry: {version}");
                return version;
            }
            else
            {
                Debug.WriteLine("Version key not found in registry. Returning default version value.");
                return string.Empty; // or return a default value
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., problems accessing the registry)
            Debug.WriteLine($"An error occurred while accessing the registry: {ex.Message}");
            return string.Empty; // or return a default value in case of an error
        }
    }

    private async Task PerformUpdate(string downloadUrl)
    {
        var downloadedFilePath = await _updateService.DownloadUpdateAsync(downloadUrl);
        Debug.WriteLine($"Downloaded file path: {downloadedFilePath}");
        _updateService.InstallUpdate(downloadedFilePath);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
