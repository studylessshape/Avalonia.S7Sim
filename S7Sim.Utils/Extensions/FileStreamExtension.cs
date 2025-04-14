using System.IO;
using System.Text;

namespace S7Sim.Utils.Extensions
{
    public static class FileStreamExtension
    {
        public static void WriteString(this FileStream stream, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            stream.Write(Encoding.UTF8.GetBytes(value));
        }
    }
}
