using Intune_Group_Assignments.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Intune_Group_Assignments.Views
{
    public sealed partial class AllDataPage : Page
    {
        public AllDataPage()
        {
            this.InitializeComponent();
            // Initialisez votre ViewModel ici si nécessaire
            this.DataContext = new AllDataViewModel();
        }
    }
}