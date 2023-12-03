using System.Diagnostics;
using System.Threading.Tasks;
using Intune_Deployment_Monitor.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Markdig;

namespace Intune_Deployment_Monitor.ViewModels
{
    internal class WhatsNewViewModel : ObservableObject
    {
        private string _latestReleaseBody;
        private string _htmlContent;

        public string LatestReleaseBody
        {
            get => _latestReleaseBody;
            set => SetProperty(ref _latestReleaseBody, value);
        }

        public string HtmlContent
        {
            get => _htmlContent;
            set => SetProperty(ref _htmlContent, value);
        }

        public WhatsNewViewModel()
        {
            Debug.WriteLine("Loading latest release info from GitHub API...");
            LoadLatestReleaseAsync();
        }

        private async Task LoadLatestReleaseAsync()
        {
            LatestReleaseBody = await WhatsNewService.GetLatestReleaseInfoAsync();
            Debug.WriteLine($"Latest release body: {LatestReleaseBody}");

            HtmlContent = Markdown.ToHtml(LatestReleaseBody);
            Debug.WriteLine($"HTML content: {HtmlContent}");
        }
    }
}
