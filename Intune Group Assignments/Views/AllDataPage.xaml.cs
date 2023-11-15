using Intune_Group_Assignments.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Intune_Group_Assignments.Views;

public sealed partial class AllDataPage : Page
{
    public AllDataViewModel ViewModel
    {
        get;
    }

    public AllDataPage()
    {
        ViewModel = App.GetService<AllDataViewModel>();
        InitializeComponent();
        this.DataContext = ViewModel;
    }
}