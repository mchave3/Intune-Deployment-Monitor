using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Intune_Deployment_Monitor.Models;
using System.Linq;
using System.Collections.Generic;

namespace Intune_Deployment_Monitor.ViewModels
{
    public class AllDataViewModel : ObservableObject
    {
        private readonly AllDataModel _allDataModel;
        private List<DataAssignment> _allDataAssignments;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ObservableCollection<DataAssignment> DataAssignments
        {
            get;
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilterDataAssignments();
            }
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
            var data = await _allDataModel.GetAllDataAsync();
            _allDataAssignments = data.Select(tuple => new DataAssignment
            {
                ResourceName = tuple.ResourceName,
                GroupId = tuple.GroupId,
                GroupDisplayName = tuple.GroupDisplayName,
                ResourceType = tuple.ResourceType,
                DeploymentStatus = tuple.DeploymentStatus,
                IncludeExcludeStatus = tuple.IncludeExcludeStatus
            }).ToList();

            DataAssignments.Clear();
            foreach (var item in _allDataAssignments)
            {
                DataAssignments.Add(item);
            }

            IsLoading = false;
        }

        private void FilterDataAssignments()
        {
            var filteredData = string.IsNullOrEmpty(SearchText)
                ? _allDataAssignments
                : _allDataAssignments.Where(da => da.ResourceName.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);

            DataAssignments.Clear();
            foreach (var item in filteredData)
            {
                DataAssignments.Add(item);
            }

            OnPropertyChanged(nameof(DataAssignments));
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
        public string DeploymentStatus
        {
            get; set;
        }
        public string IncludeExcludeStatus
        {
            get; set;
        }
    }
}
