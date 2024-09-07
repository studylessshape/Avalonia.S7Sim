using Avalonia.S7Sim.ViewModels;
using Avalonia.S7Sim.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Avalonia.S7Sim.Services
{
    internal static class RegistServicesColleciton
    {
        internal static IServiceCollection RegistViews(this IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            return services;
        }

        internal static void Regist(HostBuilderContext context, IServiceCollection services)
        {
            services.RegistViews();
        }
    }
}
