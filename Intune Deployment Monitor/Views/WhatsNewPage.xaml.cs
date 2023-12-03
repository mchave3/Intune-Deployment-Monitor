using Microsoft.UI.Xaml.Controls;
using Intune_Deployment_Monitor.ViewModels;

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
        }

        private async void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WhatsNewViewModel.HtmlContent))
            {
                await webView.EnsureCoreWebView2Async();
                webView.NavigateToString(viewModel.HtmlContent);
            }
        }
    }
}
