using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Intune_Group_Assignments.Models;
using CommunityToolkit.WinUI.UI.Controls;

namespace Intune_Group_Assignments.ViewModels
{
    public class AllDataViewModel : ObservableObject
    {
        private readonly AllDataModel _allDataModel;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

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
            _allDataModel = new AllDataModel();
            PolicyAssignments = new ObservableCollection<PolicyAssignment>();
            RefreshCommand = new RelayCommand(LoadDataAsync);
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            IsLoading = true;
            PolicyAssignments.Clear();

            var data = await _allDataModel.GetAllDataAsync();
            foreach (var item in data)
            {
                PolicyAssignments.Add(new PolicyAssignment
                {
                    PolicyName = item.PolicyName,
                    GroupId = item.GroupId,
                    ResourceName = item.ResourceName
                });
            }
            IsLoading = false;
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
