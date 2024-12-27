using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PipeProtocol
{
    public class PipeCommand
    {
        public string Module { get; set; }
        public string Method { get; set; }
        public string[] Parameters { get; set; }

        public string ToCommand()
        {
            string para = string.Join(',', Parameters.Select(para =>
            {
                if (para.Contains(','))
                {
                    return $"\"{para.Replace("\"", "\\\"")}\"";
                }
                return para;
            }));

            return $"{Module}.{Method} {para}";
        }

        public static PipeCommand Parse(string receive)
        {
            var module = receive[0..receive.IndexOf('.')];
            var indexOfMethod = receive.IndexOf(' ');
            var method = receive[(receive.IndexOf(".") + 1)..indexOfMethod];

            var splits = receive[(indexOfMethod + 1)..].Split(',');

            List<string> parameters = new List<string>();

            StringBuilder builder = new StringBuilder();
            bool isCommaClose = true;
            foreach (var para in splits)
            {
                var para_trim_start = para.TrimStart();
                if (isCommaClose && !para_trim_start.StartsWith('"'))
                {
                    parameters.Add(para);
                }
                else if (isCommaClose)
                {
                    builder.Clear();
                    var para_trim_end = para.TrimEnd();
                    if (para_trim_end.EndsWith('"') && !para_trim_end.EndsWith("\\\""))
                    {
                        parameters.Add(para_trim_end.TrimStart());
                    }
                    else
                    {
                        isCommaClose = false;
                        builder.Append(para_trim_start);
                    }
                }
                else
                {
                    var para_trim_end = para.TrimEnd();
                    if (para_trim_end.EndsWith('"') && !para_trim_end.EndsWith("\\\""))
                    {
                        builder.Append("," + para_trim_end);
                        parameters.Add(builder.ToString());
                        isCommaClose = true;
                        builder.Clear();
                    }
                    else
                    {
                        builder.Append("," + para);
                    }
                }
            }

            return new PipeCommand()
            {
                Module = module,
                Method = method,
                Parameters = parameters.ToArray()
            };
        }
    }
}
