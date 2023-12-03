using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Intune_Deployment_Monitor.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Intune_Deployment_Monitor.ViewModels
{
    internal class WhatsNewViewModel : ObservableObject
    {
        private readonly WhatsNewService _whatsNewService;
        private ReleaseInfo _latestRelease;

        public ReleaseInfo LatestRelease
        {
            get => _latestRelease;
            set => SetProperty(ref _latestRelease, value);
        }

        public WhatsNewViewModel()
        {
            _whatsNewService = new WhatsNewService();
            LoadLatestReleaseAsync();
        }

        private async Task LoadLatestReleaseAsync()
        {
            LatestRelease = await _whatsNewService.GetLatestReleaseInfoAsync();
        }
    }

    public class ReleaseInfo
    {
        public string TagName
        {
            get; set;
        }
        public string Body
        {
            get; set;
        }
        // Ajoutez d'autres propriétés selon les données de release que vous souhaitez afficher
    }
}
