using System.Diagnostics;
using Intune_Deployment_Monitor.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Intune_Deployment_Monitor.ViewModels
{
    internal class WhatsNewViewModel : ObservableObject
    {
        private string _latestReleaseBody;

        public string LatestReleaseBody
        {
            get => _latestReleaseBody;
            set => SetProperty(ref _latestReleaseBody, value);
        }

        public WhatsNewViewModel()
        {
            Debug.WriteLine("Loading latest release info from GitHub API...");
            LoadLatestReleaseAsync();
        }

        // Load the latest release info from GitHub API
        private async Task<string> LoadLatestReleaseAsync()
        {
            // Get the latest release info from GitHub API
            LatestReleaseBody = await WhatsNewService.GetLatestReleaseInfoAsync();
            Debug.WriteLine($"Latest release body: {LatestReleaseBody}");

            return LatestReleaseBody;
        }
    }
}
