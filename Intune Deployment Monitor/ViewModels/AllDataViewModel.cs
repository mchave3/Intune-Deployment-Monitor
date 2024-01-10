using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Intune_Deployment_Monitor.Models;
using System.Text;
using WinRT.Interop;

namespace Intune_Deployment_Monitor.ViewModels
{
    // ViewModel for all data
    public class AllDataViewModel : ObservableObject
    {
        // Model for all data
        private readonly AllDataModel _allDataModel;
        // List of all data assignments
        private List<DataAssignment> _allDataAssignments;

        // Property indicating if data is loading
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Collection of data assignments
        public ObservableCollection<DataAssignment> DataAssignments
        {
            get;
        }

        // Text for searching data assignments
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

        // Command to refresh data
        public RelayCommand RefreshCommand
        {
            get;
        }

        // Command to export data to CSV
        public RelayCommand ExportToCsvCommand
        {
            get;
        }

        // Constructor
        public AllDataViewModel()
        {
            _allDataModel = new AllDataModel();
            DataAssignments = new ObservableCollection<DataAssignment>();
            RefreshCommand = new RelayCommand(LoadDataAsync);
            ExportToCsvCommand = new RelayCommand(ExportDataToCsv);
            LoadDataAsync();
        }

        // Asynchronously load data
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

        // Filter data assignments based on search text
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

        // Export data to CSV file
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
}


// Class representing a data assignment
public class DataAssignment
{
    // Type of the resource
    public string ResourceType
    {
        get; set;
    }
    // ID of the group
    public string GroupId
    {
        get; set;
    }
    // Display name of the group
    public string GroupDisplayName
    {
        get; set;
    }
    // Name of the resource
    public string ResourceName
    {
        get; set;
    }
    // Status of the deployment
    public string DeploymentStatus
    {
        get; set;
    }
    // Status of include/exclude
    public string IncludeExcludeStatus
    {
        get; set;
    }
}