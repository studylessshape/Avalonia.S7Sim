using Avalonia.Controls.Notifications;
using Microsoft.Extensions.Hosting;
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
    public class PipeHost : BackgroundService
    {
        private readonly PipeProfiles profiles;
        private readonly IShellCommand shell;

        private Dictionary<string, object> commands = [];

        public PipeHost(PipeProfiles profiles, IShellCommand shell, IS7DataBlockService dbService, IS7MBService mbService)
        {
            this.profiles = profiles;
            this.shell = shell;
            RegistCommand("shell", shell);
            RegistCommand("DB", dbService);
            RegistCommand("MB", mbService);
        }

        private void RegistCommand(string name, object module)
        {
            ArgumentNullException.ThrowIfNull(module);

            if (!commands.TryAdd(name, module))
            {
                commands[name] = module;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // 开启服务管道
                try
                {
                    using var recevieStream = new NamedPipeServerStream(profiles.PipeName);
                    await recevieStream.WaitForConnectionAsync(stoppingToken);
                    if (recevieStream.IsConnected)
                    {
                        var command = await ProtocolTools.ReadCommandAsync(recevieStream, stoppingToken);
                        await ProtocolTools.SendResponseAsync(recevieStream, RunCommand(command), stoppingToken);
                    }
                }
                catch (Exception e)
                {
                    shell.SendLogMessage($"Occurs error on NamedPipe:\n{e.Message}", (int)NotificationType.Error);
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

                object?[] parameters = new object[command.Parameters.Length];
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
                    ErrCode = (int)ErrCodes.WhenBuildParameters,
                    Message = e.Message
                };
            }
        }

        private object? ParseParameter(Type paraType, string paraStr)
        {
            var method = paraType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, [typeof(string)]);
            return method == null ? throw new Exception("Only support primary type") : method.Invoke(paraType, [paraStr]);
        }
    }
}
