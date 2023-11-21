using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System.Xml;
using System.IO;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

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
        // Path to the executable
        string exePath = @"C:\Program Files\Mickael CHAVE\Intune Group Assignments\Intune Group Assignments.exe";

        Debug.WriteLine($"Checking version for executable at: {exePath}");

        // Check if the file exists
        if (File.Exists(exePath))
        {
            Debug.WriteLine("Executable found. Retrieving file version information...");

            // Get the version information of the file
            var versionInfo = FileVersionInfo.GetVersionInfo(exePath);

            // Return the file version
            string version = versionInfo.FileVersion;
            Debug.WriteLine($"File version retrieved: {version}");
            return version;
        }
        else
        {
            // Handle the case where the file does not exist
            Debug.WriteLine("Executable not found. Returning default version value.");

            // For example, return an empty string or a default value
            return string.Empty;
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
