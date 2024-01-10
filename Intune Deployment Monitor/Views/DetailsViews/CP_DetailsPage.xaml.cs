using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Intune_Deployment_Monitor.Behaviors;

namespace Intune_Deployment_Monitor.Views.DetailsViews;

public sealed partial class CP_DetailsPage : Page
{
    public CP_DetailsPage()
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
