using Avalonia.Platform.Storage;
using Avalonia.S7Sim.Messages;
using Avalonia.S7Sim.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.ViewModels;

public partial class PyEngineViewModel : ViewModelBase
{
#if DEBUG
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    public PyEngineViewModel()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    {

    }
#endif

    private readonly PyScriptRunner _pyRunner;
    private readonly IServiceProvider serviceProvider;

    public ObservableCollection<string> PyEngineSearchPaths { get; } = new();

    public PyEngineViewModel(PyScriptRunner pyRunner, IServiceProvider serviceProvider)
    {
        this._pyRunner = pyRunner;
        this.serviceProvider = serviceProvider;
        var searchPaths = _pyRunner.PyEngine.GetSearchPaths();
        if (searchPaths != null)
        {
            PyEngineSearchPaths.AddRange(searchPaths);
        }
        PyEngineSearchPaths.CollectionChanged += PyEngineSearchPaths_CollectionChanged;
    }

    private void PyEngineSearchPaths_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            _pyRunner.PyEngine.SetSearchPaths(e.NewItems.Select(obj => (string)obj).ToList());
        }
    }

    [RelayCommand]
    private async Task SelectPath()
    {
        try
        {
            var folders = await serviceProvider.GetRequiredService<IStorageProvider>().OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                AllowMultiple = true,
            });

            if (folders is not null)
            {
                PyEngineSearchPaths.AddRange(folders.Where(f => f != null).Select(f => f.Path.AbsolutePath));
            }
        }
        catch (System.Exception ex)
        {
            MessageHelper.ShowMessage(new MessageContent()
            {
                Message = $"打开路径出现错误！\n{ex.Message}"
            });
        }
    }
}
