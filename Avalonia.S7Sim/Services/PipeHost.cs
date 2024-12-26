using Avalonia.Controls.Notifications;
using Microsoft.Extensions.Hosting;
using S7Sim.Services;
using System;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.Services
{
    public class PipeHost : BackgroundService
    {
        private readonly PipeProfiles profiles;
        private readonly IShellCommand shell;

        public PipeHost(PipeProfiles profiles, IShellCommand shell)
        {
            this.profiles = profiles;
            this.shell = shell;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var methods = shell.GetType().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod);
            if (methods != null)
            {
                var method = methods.Where(m => m.Name == "dsadsa").First();
                var paras = method.GetParameters();
                method.Invoke(shell, new object[]
                {
                });
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                // 开启服务管道
                try
                {
                    using var recevieStream = new NamedPipeServerStream(profiles.PipeName);
                    await recevieStream.WaitForConnectionAsync(stoppingToken);
                    if (recevieStream.IsConnected)
                    {
                        StringBuilder read = new();

                        // 读取所有的内容
                        while(recevieStream.CanRead)
                        {
                            var buffer = new byte[1024].AsMemory(0, 1024);
                            // 读取管道流中的内容，注意不能使用 StreamReader，会读取到空内容
                            var readLength = await recevieStream.ReadAsync(buffer, stoppingToken);
                            if (readLength > 0)
                            {
                                // byte 设置的大小为 1024，空余的内容会在解码时自动补 \0，所以需要去除
                                var content = Encoding.UTF8.GetString(buffer[..readLength].ToArray()).TrimEnd();
                                if (content != null)
                                {
                                    read.Append(content);
                                }
                            }
                        }

                        //if (content == "Open")
                        //{
                        //    NotifyIconViewModel.ShowMainWindow();
                        //}
                    }
                }
                catch (Exception e)
                {
                    shell.SendLogMessage($"Occurs error on NamedPipe: {e}", (int)NotificationType.Error);
                }
            }
        }
    }
}
