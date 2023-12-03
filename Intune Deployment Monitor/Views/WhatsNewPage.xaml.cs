using Microsoft.UI.Xaml.Controls;
using Intune_Deployment_Monitor.ViewModels;

namespace Intune_Deployment_Monitor.Views
{
    public sealed partial class WhatsNewPage : Page
    {
        public WhatsNewPage()
        {
            this.InitializeComponent();
            this.Loaded += WhatsNewPage_Loaded;
        }

        private async void WhatsNewPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var viewModel = DataContext as WhatsNewViewModel;
            if (viewModel != null)
            {
                await webView.EnsureCoreWebView2Async();
                webView.NavigateToString(viewModel.HtmlContent);
            }
        }
    }
}
