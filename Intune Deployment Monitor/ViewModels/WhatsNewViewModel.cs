using System.Diagnostics;
using Intune_Deployment_Monitor.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Markdig;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Markup;

namespace Intune_Deployment_Monitor.ViewModels
{
    internal class WhatsNewViewModel : ObservableObject
    {
        private string _latestReleaseBody;
        private Paragraph _richTextContent;

        public string LatestReleaseBody
        {
            get => _latestReleaseBody;
            set => SetProperty(ref _latestReleaseBody, value);
        }

        public Paragraph RichTextContent
        {
            get => _richTextContent;
            set => SetProperty(ref _richTextContent, value);
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

            RichTextContent = ConvertMarkdownToRichText(LatestReleaseBody);
            Debug.WriteLine($"RichTextContent created");
        }

        private Paragraph ConvertMarkdownToRichText(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var markdownResult = Markdig.Markdown.ToHtml(markdown, pipeline);

            var xaml = $@"<Paragraph xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>{markdownResult}</Paragraph>";
            return XamlReader.Load(xaml) as Paragraph;
        }
    }
}
