using Intune_Group_Assignments.ViewModels;
using Intune_Group_Assignments.Behaviors;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Intune_Group_Assignments.Views
{
    public sealed partial class DetailsPage : Page
    {
        public DetailsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var data = e.Parameter as DataAssignment;

            if (data != null)
            {
                ResourceTypeTextBlock.Text += " " + data.ResourceType;
                GroupIdTextBlock.Text += " " + data.GroupId;
                GroupDisplayNameTextBlock.Text += " " + data.GroupDisplayName;
                ResourceNameTextBlock.Text += " " + data.ResourceName;

                // Set the header context for this page
                NavigationViewHeaderBehavior.SetHeaderContext(this, data.ResourceName);
            }
        }


        // Event handler for the Return button
        private void AppBarButton_Return(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
    }
}