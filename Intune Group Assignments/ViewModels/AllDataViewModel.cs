using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Intune_Group_Assignments.ViewModels
{
    public partial class AllDataViewModel : ObservableObject
    {
        private ObservableCollection<SampleData> _sampleTable;

        public ObservableCollection<SampleData> SampleTable
        {
            get => _sampleTable;
            set => SetProperty(ref _sampleTable, value);
        }

        public AllDataViewModel()
        {
            _sampleTable = new ObservableCollection<SampleData>();

            // Add some sample data to the collection
            for (int i = 1; i <= 50; i++)
            {
                _sampleTable.Add(new SampleData { Name = $"Boat {i}", Value = $"Value {i}" });
            }

            // ... add more data as needed
        }
    }

    // Define a simple data model for the sample data
    public class SampleData
    {
        public string Name
        {
            get; set;
        }
        public string Value
        {
            get; set;
        }
    }
}
