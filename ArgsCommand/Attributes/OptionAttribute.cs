using System;

namespace ArgsCommand.Attributes
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class OptionAttribute : Attribute
    {
        /// <summary>
        /// Short option flag like <see langword="-f"/>. And only need set the <see cref="Short"/> with <see langword="f"/>
        /// </summary>
        public string Short { get; set; }
        /// <summary>
        /// Long option flag like <see langword="--file"/>. And only need set the <see cref="Long"/> with <see langword="file"/>
        /// </summary>
        public string Long { get; set; }

        public bool Required { get; set; }
    }
}
