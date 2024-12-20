using System;

namespace ArgsCommand.Attributes
{
    /// <summary>
    /// Mark defination to Command
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
    public sealed class ConsoleCommandAttribute : Attribute
    {
        /// <summary>
        /// Ignore word case, default is <see langword="true"/>
        /// </summary>
        public bool IgnoreCase { get; set; } = true;
    }
}
