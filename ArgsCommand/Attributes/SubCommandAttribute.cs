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

        /// <summary>
        /// Remove option char
        /// </summary>
        public void Normalize()
        {
            Command = Command.TrimStart('-');
        }
    }
}
