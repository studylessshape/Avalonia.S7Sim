using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Data.Converters;

namespace Avalonia.S7Sim.Converters;

public class ItemIndexConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        IList? allItems = null;

        foreach (var item in values)
        {
            if (item is IList list)
            {
                allItems = list;
                break;
            }
        }

        if (allItems == null)
        {
            return null;
        }
        
        //values.Remove(allItems);
        
        var oneItem = values.Where(o => o != allItems).FirstOrDefault();

        if (oneItem == null || !allItems.Contains(oneItem))
        {
            return null;
        }

        var idx = allItems.IndexOf(oneItem);
        return $"{idx + 1}";
    }
}