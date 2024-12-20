using System;

namespace ArgsCommand.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class SubCommandAttribute : Attribute
    {
        /// <summary>
        /// If don't set, will use property name.
        /// </summary>
        public string Command { get; set; }

        public string Description { get; set; }
    }
}
