using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Avalonia.S7Sim.Converters
{
    public class EnumToArrayConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is not Type)
            {
                throw new ArgumentException("Value muse be Enum Type", nameof(value));
            }

            var type = (Type)value;
            if (!type.IsEnum)
            {
                throw new ArgumentException("Value must be Enum Type", nameof(value));
            }

            return Enum.GetValues(type);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
