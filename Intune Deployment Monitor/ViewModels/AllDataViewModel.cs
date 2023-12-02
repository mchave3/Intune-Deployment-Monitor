using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Intune_Deployment_Monitor.Models;
using System.Text;
using WinRT.Interop;

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

        public RelayCommand ExportToCsvCommand
        {
        
            get;
        }

        public AllDataViewModel()
        {
            _allDataModel = new AllDataModel();
            DataAssignments = new ObservableCollection<DataAssignment>();
            RefreshCommand = new RelayCommand(LoadDataAsync);
            ExportToCsvCommand = new RelayCommand(ExportDataToCsv);
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

        private async void ExportDataToCsv()
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("CSV", new List<string>() { ".csv" });
            savePicker.SuggestedFileName = "ExportedData";

            // Get the current window's handle
            IntPtr hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            InitializeWithWindow.Initialize(savePicker, hwnd);

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                StringBuilder csv = new StringBuilder();
                // Add headers
                csv.AppendLine("ResourceType,GroupId,GroupDisplayName,ResourceName,DeploymentStatus,IncludeExcludeStatus");

                // Add data
                foreach (var data in DataAssignments)
                {
                    csv.AppendLine($"{data.ResourceType},{data.GroupId},{data.GroupDisplayName},{data.ResourceName},{data.DeploymentStatus},{data.IncludeExcludeStatus}");
                }

                // Save to file
                await Windows.Storage.FileIO.WriteTextAsync(file, csv.ToString());
            }
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
