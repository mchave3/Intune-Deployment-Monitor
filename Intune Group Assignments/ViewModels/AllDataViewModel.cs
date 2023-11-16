using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Data;

namespace Intune_Group_Assignments.ViewModels
{
    public class AllDataViewModel : ObservableObject
    {
        public ObservableCollection<YourDataModel> SampleData
        {
            get; private set;
        }

        public AllDataViewModel()
        {
            SampleData = new ObservableCollection<YourDataModel>();
            for (int i = 1; i <= 50; i++)
            {
                SampleData.Add(new YourDataModel
                {
                    Name = $"Name {i}",
                    Value = $"Value {i}"
                });
            }
        }
    }

    public class YourDataModel
    {
        // Définissez les propriétés de votre modèle de données ici
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