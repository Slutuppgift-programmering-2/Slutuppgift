using System;
using System.Globalization;
using System.Windows.Data;

namespace LabShortestRouteFinder.Converters
{
    public class YPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double y)
                return y + 25; // Adjust the label position below the node
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
