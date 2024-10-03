using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.S7Sim.Messages;
using Avalonia.S7Sim.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
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

    private readonly PyScriptRunner _scriptRunner;
    private readonly IServiceProvider serviceProvider;
    private bool disposedValue;

    public S7CommandViewModel(ConfigS7ServerViewModel configModel, OperationsViewModel operationsModel, PyScriptRunner scriptRunner, IServiceProvider serviceProvider)
    {
        ConfigModel = configModel;
        OperationsViewModel = operationsModel;
        this._scriptRunner = scriptRunner;
        this.serviceProvider = serviceProvider;
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

            if (files != null)
            {
                await Task.Run(() =>
                {
                    foreach (var file in files)
                    {
                        _scriptRunner.RunFile(file.Path.AbsolutePath);
                    }
                });
                MessageHelper.ShowMessage(new MessageContent { Message = "脚本执行完毕！", Icon = MessageBoxIcon.Success });
            }
        }
        catch (Exception ex)
        {
            MessageHelper.ShowMessage(new MessageContent { Message = $"执行脚本出错！\n{ex.Message}", Icon = MessageBoxIcon.Error });
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                
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
