using Avalonia.S7Sim.Services.Shell;
using Avalonia.S7Sim.ViewModels;
using Avalonia.S7Sim.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using S7Sim.Services;
using S7Sim.Services.Scripts;
using System;
using System.Threading;

namespace Avalonia.S7Sim.Services;

internal static class RegistServicesColleciton
{
    internal static IServiceCollection RegistViews(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton(provider =>
        {
            return provider.GetRequiredService<MainWindow>().StorageProvider;
        });

        services.AddSingleton<ConfigS7ServerView>();
        services.AddSingleton<ConfigS7ServerViewModel>();

        services.AddSingleton<LogPanel>();
        services.AddSingleton<LogPanelViewModel>();

        services.AddSingleton<S7CommandView>();
        services.AddSingleton<S7CommandViewModel>();

        services.AddScoped<OperationsViewModel>();
        services.AddSingleton<MessageBoxViewModel>();

        services.AddSingleton<ScriptsView>();
        services.AddSingleton<ScriptsViewModel>();

        //services.AddSingleton<RealtimeView>();
        //services.AddSingleton<RealtimeViewModel>();

        services.AddScoped<IShellCommand, ShellCommand>();

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

    internal static void RunHostService(this IServiceProvider serviceProvider, CancellationToken stopToken)
    {
        var host = serviceProvider.GetService<PipeHost>();
        host?.StartAsync(stopToken);
    }

    internal static void Regist(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<IS7ServerService, S7ServerService>();
        services.AddSingleton<IS7DataBlockService, S7DataBlockService>();
        services.AddSingleton<IS7MBService, S7MBService>();

        services.AddSingleton<IScriptRunner, PyScriptRunner>();

        services.AddSingleton<PipeProfiles>();

        services.RegistViews();

        services.AddHostedService<PipeHost>();
    }
}
