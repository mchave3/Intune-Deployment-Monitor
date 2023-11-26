using CommunityToolkit.WinUI.UI.Controls;
using Intune_Group_Assignments.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Intune_Group_Assignments.Views
{
    public sealed partial class AllDataPage : Page
    {
        public AllDataViewModel ViewModel
        {
            get;
        }

        public AllDataPage()
        {
            this.InitializeComponent();
            ViewModel = new AllDataViewModel();
            this.DataContext = ViewModel;
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            var selectedItem = dataGrid.SelectedItem as DataAssignment;

            if (selectedItem != null)
            {
                Frame.Navigate(typeof(DetailsPage), selectedItem);
            }
        }
    }
}