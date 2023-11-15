using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Intune_Group_Assignments.ViewModels;

public partial class AllDataViewModel : ObservableRecipient
{
    public ObservableCollection<SampleData> SampleTable
    {
        get; set;
    }

    public AllDataViewModel()
    {
        // Initialize the ObservableCollection
        SampleTable = new ObservableCollection<SampleData>();

        // Add some sample data to the collection
        SampleTable.Add(new SampleData { Name = "Boat 1", Value = "Value 1" });
        SampleTable.Add(new SampleData { Name = "Boat 2", Value = "Value 2" });
        SampleTable.Add(new SampleData { Name = "Boat 3", Value = "Value 3" });
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