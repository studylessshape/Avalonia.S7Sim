using System;
using System.Linq;

namespace ArgsCommand.Attributes
{
    /// <summary>
    /// When property is <see cref="bool"/>, input the option don't need args. When input this option, will set the value of <see langword="true"/>
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class OptionAttribute : Attribute
    {
        public readonly static char[] UnsupportedChars = new char[] { '\'', '"', '&', '|' };
        /// <summary>
        /// Short option flag like <see langword="-f"/>. And only need set the <see cref="Short"/> with <see langword="f"/>
        /// </summary>
        public string Short { get; set; }
        /// <summary>
        /// Long option flag like <see langword="--file"/>. And only need set the <see cref="Long"/> with <see langword="file"/>
        /// </summary>
        public string Long { get; set; }

        /// <summary>
        /// If this option must be input. Default is <see langword="true"/>
        /// </summary>
        public bool Required { get; set; } = true;

        /// <summary>
        /// The index of parameter behind this option
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// <para>Remove whitespace and some unsupported whitespace charactor</para>
        /// <para>Then insert <c>-</c> on start position</para>
        /// </summary>
        public void Normalize()
        {
            if (Short != null)
            {
                Short = string.Join("", Short.Split(Short.Where(x => char.IsWhiteSpace(x)).Union(UnsupportedChars).ToArray())).TrimStart('-');
                Short = $"-{Short}";
            }
            if (Long != null)
            {
                Long = string.Join("", Long.Split(Long.Where(x => char.IsWhiteSpace(x)).Union(UnsupportedChars).ToArray())).TrimStart('-');
                Long = $"--{Long}";
            }
        }
    }
}
