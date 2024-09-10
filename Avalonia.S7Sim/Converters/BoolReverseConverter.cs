using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Avalonia.S7Sim.Converters;

public class BoolReverseConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || targetType != typeof(bool))
        {
            return BindingNotification.UnsetValue;
        }

        if (value is not bool)
        {
            return BindingNotification.UnsetValue;
        }

        return !(bool)value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Convert(value, targetType, parameter, culture);
    }
}
