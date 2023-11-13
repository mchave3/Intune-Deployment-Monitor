using Intune_Group_Assignments.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Intune_Group_Assignments.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }
}
