using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Data;

namespace Intune_Group_Assignments.ViewModels
{
    public class AllDataViewModel
    {
        public ICollectionView MyCollectionViewSource
        {
            get; set;
        }

        public AllDataViewModel()
        {
            // Initialisez et peuplez MyCollectionViewSource avec vos données
            // Exemple :
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