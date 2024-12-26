using ArgsCommand.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArgsCommand
{
    public static class ArgsParser
    {
        public static T Parse<T>(params string[] args)
        {
            var type = typeof(T);
            var commandAttr = type.GetCustomAttribute<ConsoleCommandAttribute>();
            var properties = type.GetProperties(BindingFlags.GetProperty | BindingFlags.SetProperty);

            Dictionary<PropertyInfo, OptionAttribute> optionAttributes;
            Dictionary<PropertyInfo, SubCommandAttribute> subCommandAttributes;

            if (commandAttr != null)
            {
                optionAttributes = properties.Select(p =>
                {
                    var subCommandAttr = p.GetCustomAttribute<SubCommandAttribute>();
                    var optionAttr = p.GetCustomAttribute<OptionAttribute>();
                    if (subCommandAttr != null && optionAttr != null)
                    {
                        throw new NotSupportedException("Can't make parameter to subcommand and option");
                    }
                    var propertyType = p.PropertyType;
                    //var isPrimitiveType = propertyType.IsPrimitive || propertyType.Equals(typeof(string)) || propertyType.Equals(typeof(DateTime));
                    if (optionAttr == null)
                    {
                        optionAttr = new OptionAttribute()
                        {
                            Long = p.Name.TrimStart('-', '_').Replace('_', '-'),
                        };
                    }
                    if (string.IsNullOrWhiteSpace(optionAttr.Short) && string.IsNullOrWhiteSpace(optionAttr.Long))
                    {
                        optionAttr.Long = p.Name.TrimStart('-', '_').Replace('_', '-');
                    }

                    optionAttr.Normalize();

                    return (p, attr: optionAttr);
                }).Where(kv => kv.attr != null).ToDictionary(kv => kv.p, kv => kv.attr);
            }
            else
            {
                optionAttributes = properties.Select(p => (p, attr: p.GetCustomAttribute<OptionAttribute>()))
                    .Where(i => i.attr != null)
                    .ToDictionary(i => i.p, i => i.attr);
            }

            subCommandAttributes = properties.Where(p => p.GetCustomAttribute<OptionAttribute>() == null && p.GetCustomAttribute<SubCommandAttribute>() != null).Select(p =>
            {
                var subCommandAttr = p.GetCustomAttribute<SubCommandAttribute>();
                if (string.IsNullOrWhiteSpace(subCommandAttr.Command))
                {
                    subCommandAttr.Command = p.Name.TrimStart('-', '_').Replace('_', '-');
                }

                subCommandAttr.Normalize();

                return (p, attr: subCommandAttr);
            }).ToDictionary(kv => kv.p, kv => kv.attr);

            var queryOptionTable = optionAttributes.Select((item, idx) => new { Id = idx, item.Value.Short, item.Value.Long });

            var modeComp = commandAttr.IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            var distinct = queryOptionTable.Any(q => queryOptionTable.Any(qi => qi.Id != q.Id
                && ((q.Short != null && (q.Short.Equals(qi.Short, modeComp) || q.Short.Equals(qi.Long, modeComp)))
                    || (q.Long != null && (q.Long.Equals(qi.Short, modeComp) || q.Long.Equals(qi.Long, modeComp))))));
            if (distinct)
            {
                throw new NotSupportedException("Don't support duplicate option flags");
            }

            var rootCmd = NodeCommand.ReadArgs(args);
            var result = Activator.CreateInstance(type);

            return (T)result;
        }
    }
}
