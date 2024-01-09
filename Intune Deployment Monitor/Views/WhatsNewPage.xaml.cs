using Microsoft.UI.Xaml.Controls;
using Intune_Deployment_Monitor.ViewModels;
using Microsoft.UI.Xaml.Documents;

namespace Intune_Deployment_Monitor.Views
{
    public sealed partial class WhatsNewPage : Page
    {
        private WhatsNewViewModel viewModel;

        public WhatsNewPage()
        {
            this.InitializeComponent();
            viewModel = new WhatsNewViewModel();
            this.DataContext = viewModel;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Declare and initialize the RichTextBlock
            RichTextBlock richTextBlock = new RichTextBlock();

            // Assign the RichTextBlock content
            richTextBlock.Blocks.Add(viewModel.RichTextContent);
        }

        private async void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WhatsNewViewModel.RichTextContent))
            {
                // Update the RichTextBlock content
                richTextBlock.Blocks.Clear();
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(item: new Run { Text = viewModel.RichTextContent });
                richTextBlock.Blocks.Add(paragraph);
            }
        }
    }
}
