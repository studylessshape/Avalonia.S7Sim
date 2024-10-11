using Avalonia.Platform.Storage;
using Avalonia.S7Sim.Messages;
using Avalonia.S7Sim.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

    public ObservableCollection<PathForView> PyEngineSearchPaths { get; } = new();

    public PyEngineViewModel(PyScriptRunner pyRunner, IServiceProvider serviceProvider)
    {
        this._pyRunner = pyRunner;
        this.serviceProvider = serviceProvider;
        var searchPaths = _pyRunner.PyEngine.GetSearchPaths();
        if (searchPaths != null)
        {
            PyEngineSearchPaths.AddRange(searchPaths.Select(p => new PathForView { Path = p, CanDelete = false }));
        }
        PyEngineSearchPaths.CollectionChanged += PyEngineSearchPaths_CollectionChanged;

        if (PyEngineSearchPaths.Count <= 1)
        {
            var processPath = Path.GetDirectoryName(Environment.ProcessPath);
            if (!string.IsNullOrEmpty(processPath))
            {
                PyEngineSearchPaths.AddRange([new PathForView()
                {
                    Path = Path.Combine(processPath, "lib"),
                    CanDelete = false
                },
                new PathForView()
                {
                    Path = Path.Combine(processPath, "DLLs"),
                    CanDelete = false
                }]);
            }
        }
    }

    private void PyEngineSearchPaths_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            _pyRunner.PyEngine.SetSearchPaths(e.NewItems.Select(obj => obj).Where(obj => obj is PathForView).Select(p => ((PathForView)p).Path).ToList());
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

            if (folders is not null && folders.Count > 0)
            {
                PyEngineSearchPaths.AddRange(folders.Where(f => f != null).Select(f => new PathForView() { Path = f.Path.AbsolutePath, CanDelete = true }).Distinct().Where(f => !PyEngineSearchPaths.Contains(f)));
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


public struct PathForView : IEquatable<PathForView>
{
    public string Path { get; set; }
    public bool CanDelete { get; set; }

    public bool Equals(PathForView other)
    {
        return other.Path == this.Path && other.CanDelete == this.CanDelete;
    }

    public override bool Equals(object? obj)
    {
        return obj is PathForView path && Equals(path);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Path, CanDelete);
    }

    public static bool operator ==(PathForView left, PathForView right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PathForView left, PathForView right)
    {
        return !(left == right);
    }
}