using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Avalonia.S7Sim.Converters
{
    public class ValueSubstractConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null)
            {
                return BindingNotification.UnsetValue;
            }
            var newValues = values.Where(v => v != null).Select(v => (double)v);
            if (newValues.Any() is false)
            {
                return BindingNotification.UnsetValue;
            }

            var result = newValues.First();
            foreach (var v in newValues.Skip(1))
            {
                result -= v;
            }

            return result;
        }
    }
}
