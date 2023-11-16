using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Data;

namespace Intune_Group_Assignments.ViewModels
{
    public class AllDataViewModel : ObservableObject
    {
        private ICollectionView _myCollectionViewSource;

        public ICollectionView MyCollectionViewSource
        {
            get => _myCollectionViewSource;
            set => SetProperty(ref _myCollectionViewSource, value);
        }

        public AllDataViewModel()
        {
            // Initialize and populate MyCollectionViewSource with your data
            var data = new ObservableCollection<YourDataModel>();

            // Add some sample data to the collection
            for (int i = 1; i <= 50; i++)
            {
                data.Add(new YourDataModel
                {
                    Name = $"Name {i}",
                    Value = $"Value {i}"
                });
            }

            MyCollectionViewSource = new CollectionViewSource { Source = data }.View;
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