using System.Text;

namespace PythonRun
{
    internal static class ArrayExtension
    {
        internal static string AsString(this Array array, string spector)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                if (i < array.Length - 1)
                {
                    builder.Append($"{array.GetValue(i)}{spector}");
                }
                else
                {
                    builder.Append($"{array.GetValue(i)}");
                }
            }
            return builder.ToString();
        }
    }
}
