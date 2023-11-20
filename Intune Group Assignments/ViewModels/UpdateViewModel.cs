using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System.Xml;
using System.IO;

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
        CheckForUpdatesCommand = new RelayCommand(async () => await CheckForUpdates());
    }

    private async Task CheckForUpdates()
    {
        try
        {
            var latestVersion = await _updateService.CheckForUpdatesAsync();
            var currentVersion = GetCurrentVersion();

            if (IsNewVersionAvailable(currentVersion, latestVersion))
            {
                LatestVersion = latestVersion;
                var dialog = new ContentDialog
                {
                    Title = "Update Available",
                    Content = $"A new version {latestVersion} is available. Would you like to update now?",
                    PrimaryButtonText = "Yes",
                    SecondaryButtonText = "Cancel"
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    await PerformUpdate(latestVersion);
                }
            }
        }
        catch (Exception)
        {
            var errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = "An error occurred while checking for updates.",
                CloseButtonText = "Ok"
            };

            await errorDialog.ShowAsync();
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
        var manifestPath = Path.Combine(Environment.CurrentDirectory, "Package.appxmanifest");
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(manifestPath);
        var identityNode = xmlDoc.DocumentElement.SelectSingleNode("/Package/Identity");
        var version = identityNode.Attributes["Version"].Value;
        return version;
    }

    private async Task PerformUpdate(string latestVersion)
    {
        var downloadUrl = _updateService.GetDownloadUrl(latestVersion);
        var downloadedFilePath = await _updateService.DownloadUpdateAsync(downloadUrl);
        _updateService.InstallUpdate(downloadedFilePath);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
