using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.S7Sim.Messages;
using Avalonia.S7Sim.Services;
using Avalonia.S7Sim.Views;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using S7Sim.Services.Scripts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ursa.Controls;

namespace Avalonia.S7Sim.ViewModels;

public partial class S7CommandViewModel : ViewModelBase, IDisposable
{
#if DEBUG
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    public S7CommandViewModel()
#pragma warning restore CS8618
    {

    }
#endif

    private readonly IScriptRunner _scriptRunner;
    private readonly IServiceProvider serviceProvider;
    private readonly PipeProfiles pipeProfiles;
    private bool disposedValue;
    private List<Window> scriptWindow = [];
    private readonly ScriptsViewModel scriptsViewModel;

    public S7CommandViewModel(ConfigS7ServerViewModel configModel, OperationsViewModel operationsModel, ScriptsViewModel scriptsViewModel,
                              IScriptRunner scriptRunner, IServiceProvider serviceProvider, PipeProfiles pipeProfiles)
    {
        ConfigModel = configModel;
        OperationsViewModel = operationsModel;
        this.scriptsViewModel = scriptsViewModel;
        this._scriptRunner = scriptRunner;
        this.serviceProvider = serviceProvider;
        this.pipeProfiles = pipeProfiles;
    }

    public ConfigS7ServerViewModel ConfigModel { get; }
    public OperationsViewModel OperationsViewModel { get; }

    [RelayCommand]
    private async Task RunPyScriptAsync()
    {
        try
        {
            var files = await serviceProvider.GetRequiredService<IStorageProvider>().OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择 Python 脚本",
                AllowMultiple = true,
                FileTypeFilter = [new FilePickerFileType("Python")
                    {
                        Patterns = ["*.py"],
                        MimeTypes = ["text/plain"]
                    }]
            });

            if (files != null && files.Count > 0)
            {
                await Task.Run(() =>
                {
                    foreach (var file in files)
                    {
                        //_scriptRunner.RunFile(file.Path.AbsolutePath);
                        RunFile(file.Path.AbsolutePath);
                    }
                });
                MessageHelper.SendLogMessage(new LogMessage { Message = "脚本全部开始执行！", Level = Controls.Notifications.NotificationType.Success });
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowMessage(new MessageContent { Message = $"执行脚本出错！\n{ex.Message}", Icon = MessageBoxIcon.Error });
            MessageHelper.SendLogMessage(new LogMessage { Message = $"执行脚本出错！\n{ex.Message}", Level = Controls.Notifications.NotificationType.Error });
        }
    }

    protected void RunFile(string path)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var window = new SubProcessIOWindow
            {
                Title = path
            };
            scriptWindow.Add(window);
            window.Closed += (_, _) =>
            {
                scriptWindow.Remove(window);
            };

            var viewModel = (SubProcessIOViewModel?)window.DataContext;
            viewModel?.StartScript(path);
            window.Show();
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                foreach (var window in scriptWindow.ToArray())
                {
                    window.Close();
                }
            }

            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null
            disposedValue = true;
        }
    }

    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    // ~S7CommandViewModel()
    // {
    //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
