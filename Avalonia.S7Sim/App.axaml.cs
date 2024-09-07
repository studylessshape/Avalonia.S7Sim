using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.S7Sim.Services;
using Avalonia.S7Sim.ViewModels;
using Avalonia.S7Sim.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Avalonia.S7Sim
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        private IHost? s_host;
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void HostStartUp()
        {
            s_host = Host.CreateDefaultBuilder()
                .ConfigureServices(RegistServicesColleciton.Regist)
                .Build();
            ServiceProvider = s_host.Services;
        }

        public override void OnFrameworkInitializationCompleted()
        {

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);
                HostStartUp();
                desktop.MainWindow = ServiceProvider.GetService<MainWindow>();
                desktop.Exit += async (_, _) =>
                {
                    if (s_host != null)
                    {
                        await s_host.StopAsync();
                    }
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}