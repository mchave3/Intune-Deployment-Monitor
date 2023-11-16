using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Data;
using System.Threading.Tasks;
using Intune_Group_Assignments.Services;

namespace Intune_Group_Assignments.ViewModels
{
    public class AllDataViewModel : ObservableObject
    {
        private readonly MicrosoftGraphService _microsoftGraphService;

        public ObservableCollection<PolicyAssignment> PolicyAssignments
        {
            get; private set;
        }

        public RelayCommand RefreshCommand
        {
            get;
        }

        public AllDataViewModel()
        {
            _microsoftGraphService = new MicrosoftGraphService();
            PolicyAssignments = new ObservableCollection<PolicyAssignment>();
            RefreshCommand = new RelayCommand(LoadDataAsync);
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            PolicyAssignments.Clear();  // Clear existing items before loading new data

            var data = await _microsoftGraphService.GetAllDataAsync();
            foreach (var item in data)
            {
                PolicyAssignments.Add(new PolicyAssignment
                {
                    PolicyName = item.PolicyName,
                    GroupId = item.GroupId,
                    ResourceName = item.ResourceName
                });
            }
        }
    }

    public class PolicyAssignment
    {
        public string PolicyName
        {
            get; set;
        }
        public string GroupId
        {
            get; set;
        }
        public string ResourceName
        {
            get; set;
        }
    }
}
