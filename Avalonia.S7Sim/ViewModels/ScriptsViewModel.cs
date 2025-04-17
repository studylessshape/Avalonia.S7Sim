using Avalonia.Platform.Storage;
using Avalonia.S7Sim.Messages;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Scripting.Utils;
using S7Sim.Services.Scripts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using S7Sim.Services.Models;
using S7Sim.Utils.Extensions;
using Ursa.Controls;

namespace Avalonia.S7Sim.ViewModels;

public partial class ScriptsViewModel : ViewModelBase
{
#if DEBUG
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    public ScriptsViewModel()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    {

    }
#endif
    private const string FILE_NAME = "search-path.txt";
    private readonly IScriptRunner _scriptRunner;
    private readonly IServiceProvider serviceProvider;

    public ObservableCollection<string> EngineSearchPaths { get; } = new();

    private string SavedFileName
    {
        get
        {
            return Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), FILE_NAME);
        }
    }

    public ScriptsViewModel(IScriptRunner scriptRunner, IServiceProvider serviceProvider)
    {
        this._scriptRunner = scriptRunner;
        this.serviceProvider = serviceProvider;
        var searchPaths = _scriptRunner.Engine.GetSearchPaths();
        if (searchPaths != null)
        {
            EngineSearchPaths.AddRange(searchPaths);
        }

        if (EngineSearchPaths.Count == 0)
        {
            var processPath = Path.GetDirectoryName(Environment.ProcessPath);
            if (!string.IsNullOrEmpty(processPath))
            {
                EngineSearchPaths.AddRange([
                    ".", Path.Combine(processPath, "lib"), Path.Combine(processPath, "DLLs"),
                    Path.Combine(processPath, "predefined/s7svrsim")
                ]);
            }
        }
        LoadSearchPath(SavedFileName);
        _scriptRunner.Engine.SetSearchPaths(EngineSearchPaths);
        EngineSearchPaths.CollectionChanged += PyEngineSearchPaths_CollectionChanged;
    }

    private void LoadSearchPath(string path)
    {
        if (!File.Exists(path)) return;
        try
        {
            var fileContent = File.ReadAllLines(path);
            EngineSearchPaths.Clear();
            EngineSearchPaths.AddRange(fileContent);
        }
        catch (Exception ex)
        {
            MessageHelper.ShowMessage(new MessageContent() { Message = $"加载路径出现错误！\n{ex}", Icon = MessageBoxIcon.Error });
        }
    }
    
    private void SaveServerItem(string path)
    {
        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        fileStream.WriteString(string.Join(Environment.NewLine, EngineSearchPaths));
    }

    private void PyEngineSearchPaths_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Move)
        {
            SaveServerItem(SavedFileName);
            _scriptRunner.Engine.SetSearchPaths(EngineSearchPaths);
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

            if (folders != null && folders.Count > 0)
            {
                EngineSearchPaths.AddRange(folders.Where(f => f != null).Select(f => f.Path.AbsolutePath).Distinct().Where(f => !EngineSearchPaths.Contains(f)));
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
    
    [RelayCommand]
    private void DeletePath(string path)
    {
        EngineSearchPaths.Remove(path);
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