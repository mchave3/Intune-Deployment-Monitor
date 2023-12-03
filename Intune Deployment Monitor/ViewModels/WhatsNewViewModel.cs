using System;
using System.Threading.Tasks;
using Intune_Deployment_Monitor.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Markdig;

namespace Intune_Deployment_Monitor.ViewModels
{
    internal class WhatsNewViewModel : ObservableObject
    {
        private readonly WhatsNewService _whatsNewService;
        private ReleaseInfo _latestRelease;
        private string _htmlContent;

        public ReleaseInfo LatestRelease
        {
            get => _latestRelease;
            set => SetProperty(ref _latestRelease, value);
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
            // Autres propriétés...
        }

        public string HtmlContent
        {
            get => _htmlContent;
            set => SetProperty(ref _htmlContent, value);
        }

        public WhatsNewViewModel()
        {
            _whatsNewService = new WhatsNewService();
            LoadLatestReleaseAsync();
        }

        private async Task LoadLatestReleaseAsync()
        {
            LatestRelease = await _whatsNewService.GetLatestReleaseInfoAsync();
            HtmlContent = Markdown.ToHtml(LatestRelease.Body);
        }
    }
}