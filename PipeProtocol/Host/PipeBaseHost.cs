﻿using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace PipeProtocol
{
    public abstract class PipeBaseHost : IPipeCommandHost
    {

        protected Dictionary<string, object> commandModules = new Dictionary<string, object>();

        public string PipeName { get; private set; }

        protected Thread runThread;

        public void RegistCommand(string name, object module)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            if (!commandModules.TryAdd(name, module))
            {
                commandModules[name] = module;
            }
        }

        public void RemoveCommand(string moduleName)
        {
            if (commandModules.ContainsKey(moduleName))
            {
                commandModules.Remove(moduleName);
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

        /// <summary>
        /// <para>Level:</para>
        /// 0 - Information<br/>
        /// 1 - Success<br/>
        /// 2 - Warning<br/>
        /// 3 - Error<br/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        public abstract void LogMessage(string message, int level = 0);

        protected async Task ExecuteAsync(string pipeName, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // 开启服务管道
                var pipeServerStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                try
                {
                    await pipeServerStream.WaitForConnectionAsync(stoppingToken);
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            if (pipeServerStream.IsConnected)
                            {
                                var command = await ProtocolTools.ReadCommandAsync(pipeServerStream, stoppingToken);
                                await ProtocolTools.SendResponseAsync(pipeServerStream, command.RunCommand(commandModules), stoppingToken);
                            }

                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            pipeServerStream.Close();
                            pipeServerStream.Dispose();
                        }
                    });
                }
                catch (OperationCanceledException cancelException)
                {
                    if (cancelException.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    LogMessage($"Occurs error on NamedPipe:\n{e.Message}", 3);
                }
                finally
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        pipeServerStream.Close();
                        pipeServerStream.Dispose();
                    }
                }
            }
        }

        public async Task RunOnTaskAsync(string pipeName, CancellationToken stoppingToken = default)
        {
            PipeName = pipeName;
            await ExecuteAsync(pipeName, stoppingToken);
        }
    }
}
