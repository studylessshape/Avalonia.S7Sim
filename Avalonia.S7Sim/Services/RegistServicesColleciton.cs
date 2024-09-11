using Avalonia.S7Sim.ViewModels;
using Avalonia.S7Sim.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Avalonia.S7Sim.Services;

internal static class RegistServicesColleciton
{
    internal static IServiceCollection RegistViews(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();

        services.AddSingleton<ConfigS7ServerView>();
        services.AddSingleton<ConfigS7ServerViewModel>();

        services.AddSingleton<LogPanel>();
        services.AddSingleton<LogPanelViewModel>();

        services.AddSingleton<MessageBoxViewModel>();

        return services;
    }

    /// <summary>
    /// 唤醒服务，实例化懒加载的单例
    /// </summary>
    /// <param name="serviceProvider"></param>
    internal static void WeakupService(this IServiceProvider serviceProvider)
    {
        serviceProvider.GetService<MessageBoxViewModel>();
    }

    internal static void Regist(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<IS7ServerService, S7ServerService>();
        services.AddSingleton<IS7DataBlockService, S7DataBlockService>();
        services.AddSingleton<IS7MBService, S7MBService>();

        services.RegistViews();
    }
}
