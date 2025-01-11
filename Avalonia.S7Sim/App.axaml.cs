using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.S7Sim.Services;
using Avalonia.S7Sim.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.S7Sim
{
    public partial class App : Application
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
        public IServiceProvider ServiceProvider { get; private set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。

        private IHost? s_host;
        private CancellationTokenSource stopTokenSource = new();
        private Task? _HostTask;

        public static App? AppCurrent
        {
            get
            {
                if (Current == null)
                {
                    return null;
                }

                return (App)Current;
            }
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            HostStartUp();
        }

        private void HostStartUp()
        {
            s_host = Host.CreateDefaultBuilder()
                .ConfigureServices(RegistServicesColleciton.Regist)
                .ConfigureLogging((ctx, loggerBuilder) =>
                {
                    if (ctx.HostingEnvironment.IsDevelopment())
                    {
                        loggerBuilder.AddDebug();
                    }
                })
                .Build();
            ServiceProvider = s_host.Services;
            ServiceProvider.WeakupService();
            //ServiceProvider.RunHostService(stopTokenSource.Token);
            _HostTask = Task.Run(async () =>
            {
                await s_host.RunAsync(stopTokenSource.Token);
            }, stopTokenSource.Token);
        }

        public override void OnFrameworkInitializationCompleted()
        {

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);
                desktop.MainWindow = ServiceProvider.GetService<MainWindow>();
                desktop.Exit += ExitHost;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ExitHost(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            using (s_host)
            {
                try
                {
                    var lifetime = s_host?.Services.GetRequiredService<IHostApplicationLifetime>();
                    lifetime?.StopApplication();
                    stopTokenSource.Cancel();
                }
                catch (Exception)
                {

                }
            }
        }
    }
}