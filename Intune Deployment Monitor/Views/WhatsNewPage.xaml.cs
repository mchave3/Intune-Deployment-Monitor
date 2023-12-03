using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Intune_Deployment_Monitor.Views
{
    public sealed partial class WhatsNewPage : Page
    {
        public WhatsNewPage()
        {
            this.InitializeComponent();
            this.DataContext = new ViewModels.WhatsNewViewModel();
        }
    }
}
