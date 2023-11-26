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

        public ObservableCollection<DataAssignment> DataAssignments
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
            DataAssignments = new ObservableCollection<DataAssignment>();
            RefreshCommand = new RelayCommand(LoadDataAsync);
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            IsLoading = true;
            DataAssignments.Clear();

            var data = await _allDataModel.GetAllDataAsync();
            foreach (var item in data)
            {
                DataAssignments.Add(new DataAssignment
                {
                    ResourceType = item.ResourceType,
                    GroupId = item.GroupId,
                    GroupDisplayName = item.GroupDisplayName,
                    ResourceName = item.ResourceName
                });
            }
            IsLoading = false;
        }
    }

    public class DataAssignment
    {
        public string ResourceType
        {
            get; set;
        }
        public string GroupId
        {
            get; set;
        }
        public string GroupDisplayName
        {
            get; set;
        }
        public string ResourceName
        {
            get; set;
        }
    }
}
