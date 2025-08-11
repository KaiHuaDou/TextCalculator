using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TextCalculator;
public class FontWeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => (FontWeight) value == FontWeights.Bold;
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => (bool) value ? FontWeights.Bold : FontWeights.Normal;
}
