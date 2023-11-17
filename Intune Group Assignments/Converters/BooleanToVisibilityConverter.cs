using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Intune_Group_Assignments.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}