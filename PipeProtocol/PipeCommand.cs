using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandMoudles = System.Collections.Generic.Dictionary<string, object>;

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

            List<string> parameters = new List<string>();

            if (indexOfMethod + 1 < receive.Length)
            {
                var splits = receive[(indexOfMethod + 1)..].Split(',');

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
            }

            return new PipeCommand()
            {
                Module = module,
                Method = method,
                Parameters = parameters.ToArray()
            };
        }

        public PipeResponse RunCommand(CommandMoudles commands)
        {
            if (!commands.TryGetValue(this.Module, out object module))
            {
                return new PipeResponse()
                {
                    ErrCode = (int)ErrCodes.ModuleNotFound,
                };
            }

            var method = module.GetType().GetMethod(this.Method);
            if (method == null)
            {
                return new PipeResponse()
                {
                    ErrCode = (int)ErrCodes.MethodNotFound,
                };
            }

            object[] parameters = new object[this.Parameters.Length];

            try
            {
                var methodParameters = method.GetParameters();

                var normalParamCount = methodParameters.Where(p => !p.HasDefaultValue).Count();
                var defaultParamCount = methodParameters.Where(p => p.HasDefaultValue).Count();
                var commandParamCount = this.Parameters.Length;
                if (commandParamCount > (normalParamCount + defaultParamCount))
                {
                    return new PipeResponse()
                    {
                        ErrCode = (int)ErrCodes.IncorrectParameterCount,
                    };
                }

                foreach ((var para, var index) in methodParameters.Select((p, i) => (p, i)))
                {
                    if (index >= parameters.Length)
                    {
                        break;
                    }

                    var paraStr = this.Parameters[index];
                    var paraType = para.ParameterType;

                    if (paraStr.Trim().Equals("null", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters[index] = null;
                    }
                    else if (paraType == typeof(string))
                    {
                        parameters[index] = paraStr[1..(paraStr.Length - 1)].Replace("\\\"", "\"");
                    }
                    else
                    {
                        parameters[index] = ParseParameter(paraType, paraStr);
                    }
                }
            }
            catch (Exception e)
            {
                return new PipeResponse()
                {
                    ErrCode = (int)ErrCodes.WhenBuildParameters,
                    Message = e.InnerException != null ? e.InnerException.Message : e.Message
                };
            }

            try
            {
                var returnObject = method.Invoke(module, parameters);

                var response = new PipeResponse()
                {
                    ErrCode = 0,
                };

                if (method.ReturnType != typeof(void))
                {
                    response.Message = returnObject?.ToString();
                }

                return response;
            }
            catch (Exception e)
            {
                return new PipeResponse()
                {
                    ErrCode = (int)ErrCodes.WhenRunCommand,
                    Message = e.InnerException != null ? e.InnerException.Message : e.Message
                };
            }
        }

        public static object ParseParameter(Type paraType, string paraStr)
        {
            var method = paraType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            if (method == null && paraType.Name == typeof(int?).Name)
            {
                method = paraType.GenericTypeArguments[0].GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            }
            return method == null ? throw new Exception("Only support primary type") : method.Invoke(paraType, new object[] { paraStr });
        }
    }
}
