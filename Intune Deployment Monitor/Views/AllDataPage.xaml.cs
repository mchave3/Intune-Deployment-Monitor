using CommunityToolkit.WinUI.UI.Controls;
using Intune_Deployment_Monitor.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Intune_Deployment_Monitor.Views;

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
            /*

            // If the ResourceType of the selectedItem is "Configuration Policies", we navigate to the CP_DetailsPage
            if (selectedItem.ResourceType == "Configuration Policies")
            {
                Frame.Navigate(typeof(DetailsViews.CP_DetailsPage), selectedItem);
            }

            // If the ResourceType of the selectedItem is "Device Compliance Policies", we navigate to the DCP_DetailsPage
            if (selectedItem.ResourceType == "Device Compliance Policies")
            {
                Frame.Navigate(typeof(DetailsViews.DCP_DetailsPage), selectedItem);
            }

            // If the ResourceType of the selectedItem is "Device Configurations", we navigate to the DC_DetailsPage
            if (selectedItem.ResourceType == "Device Configurations")
            {     
                Frame.Navigate(typeof(DetailsViews.DC_DetailsPage), selectedItem);
            }

            // If the ResourceType of the selectedItem is "Device Health Scripts", we navigate to the DHS_DetailsPage
            if (selectedItem.ResourceType == "Device Health Scripts")
            {                 
                Frame.Navigate(typeof(DetailsViews.DHS_DetailsPage), selectedItem);
            }

            // If the ResourceType of the selectedItem is "Device Management Scripts", we navigate to the DMS_DetailsPage
            if (selectedItem.ResourceType == "Device Management Scripts")
            {
                Frame.Navigate(typeof(DetailsViews.DMS_DetailsPage), selectedItem);
            }

            // If the ResourceType of the selectedItem is "Group Policy Configurations", we navigate to the GPC_DetailsPage
            if (selectedItem.ResourceType == "Group Policy Configurations")
            {
                Frame.Navigate(typeof(DetailsViews.GPC_DetailsPage), selectedItem);
            }

            // If the ResourceType of the selectedItem is "Mobile App Configurations", we navigate to the MAC_DetailsPage
            if (selectedItem.ResourceType == "Mobile App Configurations")
            {
                Frame.Navigate(typeof(DetailsViews.MAC_DetailsPage), selectedItem);
            }

            // If the ResourceType of the selectedItem is "Windows Autopilot Deployment Profiles", we navigate to the WADP_DetailsPage
            if (selectedItem.ResourceType == "Windows Autopilot Deployment Profiles")
            {
                Frame.Navigate(typeof(DetailsViews.WADP_DetailsPage), selectedItem);
            }

            // If the ResourceType of the selectedItem is "Applications", we navigate to the APP_DetailsPage
            if (selectedItem.ResourceType == "Applications")
            {
                Frame.Navigate(typeof(DetailsViews.APP_DetailsPage), selectedItem);
            }

            // If the ResourceType of the selectedItem is "Android Managed App Protections", we navigate to the AMAP_DetailsPage
            if (selectedItem.ResourceType == "Android Managed App Protections")
            {
                Frame.Navigate(typeof(DetailsViews.AMAP_DetailsPage), selectedItem);
            }

            // If the ResourceType of the selectedItem is "App Configurations", we navigate to the AC_DetailsPage
            if (selectedItem.ResourceType == "App Configurations")
            {
                Frame.Navigate(typeof(DetailsViews.AC_DetailsPage), selectedItem);
            }

            // If the ResourceType of the selectedItem is "iOS Managed App Protections", we navigate to the IAP_DetailsPage
            if (selectedItem.ResourceType == "iOS Managed App Protections")
            {
                Frame.Navigate(typeof(DetailsViews.IAP_DetailsPage), selectedItem);
            }

            // If the ResourceType of the selectedItem is "MDM Windows Information Protection Policies", we navigate to the MWIPP_DetailsPage
            if (selectedItem.ResourceType == "MDM Windows Information Protection Policies")
            {
                Frame.Navigate(typeof(DetailsViews.MWIPP_DetailsPage), selectedItem);
            }

            // If the ResourceType of the selectedItem is "Windows Managed App Protections", we navigate to the WMAP_DetailsPage
            if (selectedItem.ResourceType == "Windows Managed App Protections")
            {
                Frame.Navigate(typeof(DetailsViews.WMAP_DetailsPage), selectedItem);
            }

            */

            Frame.Navigate(typeof(DetailsPage), selectedItem);
        }
    }
}