using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Intune_Deployment_Monitor.Services;

namespace Intune_Deployment_Monitor.ViewModels
{
    internal class WhatsNewViewModel : INotifyPropertyChanged
    {
        private readonly WhatsNewService _whatsNewService;
        private ReleaseInfo _latestRelease;

        public event PropertyChangedEventHandler PropertyChanged;

        public ReleaseInfo LatestRelease
        {
            get => _latestRelease;
            set
            {
                _latestRelease = value;
                OnPropertyChanged(nameof(LatestRelease));
            }
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
