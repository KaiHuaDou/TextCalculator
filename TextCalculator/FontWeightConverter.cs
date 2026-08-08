using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TextCalculator;
public class FontWeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (FontWeight) value == FontWeights.Bold;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool) value ? FontWeights.Bold : FontWeights.Normal;
    }
}
