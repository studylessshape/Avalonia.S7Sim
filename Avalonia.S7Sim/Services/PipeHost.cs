using Avalonia.Controls.Notifications;
using PipeProtocol;
using S7Sim.Services;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.Services
{
    public class PipeHost : IPipeCommandHost
    {
        public IShellCommand Shell { get; }

        private Dictionary<string, object> commands = [];

        public string? PipeName { get; private set; }

        private Thread? runThread;

        public PipeHost(IShellCommand shell, IS7DataBlockService dbService, IS7MBService mbService)
        {
            this.Shell = shell;
            RegistCommand("shell", this.Shell);
            RegistCommand("DB", dbService);
            RegistCommand("MB", mbService);
        }

        public void RegistCommand(string name, object module)
        {
            ArgumentNullException.ThrowIfNull(module);

            if (!commands.TryAdd(name, module))
            {
                commands[name] = module;
            }
        }

        public void RemoveCommand(string moduleName)
        {
            if (commands.ContainsKey(moduleName))
            {
                commands.Remove(moduleName);
            }
        }

        public void RunAsync(string pipeName, CancellationToken stoppingToken = default)
        {
            PipeName = pipeName;
            runThread = new Thread(async () =>
            {
                await ExecuteAsync(pipeName, stoppingToken);
            });
            runThread.Start();
        }

        protected async Task ExecuteAsync(string pipeName, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // 开启服务管道
                try
                {
                    using var recevieStream = new NamedPipeServerStream(pipeName);
                    await recevieStream.WaitForConnectionAsync(stoppingToken);
                    if (recevieStream.IsConnected)
                    {
                        var command = await ProtocolTools.ReadCommandAsync(recevieStream, stoppingToken);
                        await ProtocolTools.SendResponseAsync(recevieStream, RunCommand(command), stoppingToken);
                    }
                }
                catch (Exception e)
                {
                    if (e is OperationCanceledException cancelException && cancelException.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    Shell.SendLogMessage($"Occurs error on NamedPipe:\n{e.Message}", (int)NotificationType.Error);
                }
            }
        }

        private PipeResponse RunCommand(PipeCommand command)
        {
            if (!commands.TryGetValue(command.Module, out object? module))
            {
                return new PipeResponse()
                {
                    ErrCode = (int)ErrCodes.ModuleNotFound,
                };
            }

            var method = module.GetType().GetMethod(command.Method);
            if (method == null)
            {
                return new PipeResponse()
                {
                    ErrCode = (int)ErrCodes.MethodNotFound,
                };
            }

            object?[] parameters = new object[command.Parameters.Length];

            try
            {
                var methodParameters = method.GetParameters();

                var normalParamCount = methodParameters.Where(p => !p.HasDefaultValue).Count();
                var defaultParamCount = methodParameters.Where(p => p.HasDefaultValue).Count();
                var commandParamCount = command.Parameters.Length;
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

                    var paraStr = command.Parameters[index];
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

        private object? ParseParameter(Type paraType, string paraStr)
        {
            var method = paraType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, [typeof(string)]);
            if (method == null && paraType.Name == typeof(int?).Name)
            {
                method = paraType.GenericTypeArguments[0].GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, [typeof(string)]);
            }
            return method == null ? throw new Exception("Only support primary type") : method.Invoke(paraType, [paraStr]);
        }
    }
}
