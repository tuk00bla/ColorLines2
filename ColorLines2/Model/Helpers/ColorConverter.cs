using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BallsAndLines.Helpers
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return (string)parameter;
            }

            switch (((Model.Ball)value).Color)
            {
                case Model.Color.Red:
                    return "Red";
                case Model.Color.Green:
                    return "Green";
                case Model.Color.Yellow:
                    return "#FFD700";
                case Model.Color.Pink:
                    return "HotPink";
                case Model.Color.Blue:
                    return "Blue"; 
                default:
                    return "Black";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
