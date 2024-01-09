using System.Diagnostics;
using Intune_Deployment_Monitor.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Markdig;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Markup;
using Windows.Data.Html;

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

        // Load the latest release info from GitHub API
        private async Task LoadLatestReleaseAsync()
        {
            // Get the latest release info from GitHub API
            LatestReleaseBody = await WhatsNewService.GetLatestReleaseInfoAsync();
            Debug.WriteLine($"Latest release body: {LatestReleaseBody}");

            // Convert the Markdown to RichText format
            RichTextContent = ConvertMarkdownToRichText(LatestReleaseBody);
            Debug.WriteLine($"RichTextContent created");
        }

        // Convert Markdown to RichText format
        private Paragraph ConvertMarkdownToRichText(string markdown)
        {
            // Convert Markdown to HTML
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var markdownResult = Markdown.ToHtml(markdown, pipeline);

            // Convert HTML to RichText
            var plainText = HtmlUtilities.ConvertToText(markdownResult);

            // Create RichTextBlock content
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run { Text = plainText });

            return paragraph;
        }
    }
}
