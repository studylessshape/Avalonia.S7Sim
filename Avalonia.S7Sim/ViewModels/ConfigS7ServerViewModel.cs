using Avalonia.Platform.Storage;
using Avalonia.S7Sim.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using S7Sim.Services;
using S7Sim.Services.Models;
using S7Sim.Utils.Extensions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ursa.Controls;

namespace Avalonia.S7Sim.ViewModels;

public partial class ConfigS7ServerViewModel : ViewModelBase
{
#if DEBUG
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    public ConfigS7ServerViewModel()
#pragma warning restore CS8618
    {

    }
#endif
    private readonly IS7ServerService _serverService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private IPAddress? _address = IPAddress.Parse("127.0.0.1");

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartServerCommand))]
    [NotifyCanExecuteChangedFor(nameof(StopServerCommand))]
    [NotifyPropertyChangedFor(nameof(CanStart))]
    [NotifyPropertyChangedFor(nameof(CanStop))]
    private bool _isServerStart;

    public bool CanStart => !IsServerStart;
    private bool CanStop => IsServerStart;

    public ObservableCollection<S7ServerItem> S7Servers { get; } = [];

    private bool CanExport => S7Servers.Any();
    private void CanExportChanged()
    {
        OnPropertyChanged(nameof(CanExport));
        ExportServerItemsCommand.NotifyCanExecuteChanged();
    }

    private string? ProcessPath { get; }
    private const string SERVER_ITEMS_SAVED_FILE = "items.csv";
    private string SavedFileName
    {
        get
        {
            if (ProcessPath != null)
            {
                return Path.Combine(ProcessPath, SERVER_ITEMS_SAVED_FILE);
            }
            else
            {
                return SERVER_ITEMS_SAVED_FILE;
            }
        }
    }

    public ConfigS7ServerViewModel(IS7ServerService serverService, IServiceProvider serviceProvider)
    {
        this._serverService = serverService;
        this._serviceProvider = serviceProvider;
        ProcessPath = Path.GetDirectoryName(Environment.ProcessPath);
        LoadServerItem();

        S7Servers.CollectionChanged += S7Servers_CollectionChanged;
    }

    private void S7Servers_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        SaveServerItem();
        if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Move)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<S7ServerItem>())
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.Cast<S7ServerItem>())
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }
    }

    private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        SaveServerItem();
    }

    private void ClearItems()
    {
        foreach (var item in S7Servers)
        {
            item.PropertyChanged -= Item_PropertyChanged;
        }
        S7Servers.Clear();
    }

    private void LoadServerItem(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                var fileContent = File.ReadAllLines(path);
                var areaKindType = typeof(AreaKind);
                foreach (var line in fileContent.Skip(1))
                {
                    var items = line.Split(',');
                    if (items.Length < 3)
                    {
                        break;
                    }

                    S7Servers.Add(new S7ServerItem()
                    {
                        AreaKind = (AreaKind)Enum.Parse(areaKindType, items[0]),
                        BlockNumber = int.Parse(items[1]),
                        BlockSize = int.Parse(items[2]),
                    });
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowMessage(new MessageContent() { Message = $"加载S7配置出现错误！\n{ex}", Icon = MessageBoxIcon.Error });
            }
        }
    }

    private void LoadServerItem()
    {
        LoadServerItem(SavedFileName);
    }

    private void SaveServerItem(string path)
    {
        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        fileStream.WriteString(string.Join(",", nameof(S7ServerItem.AreaKind), nameof(S7ServerItem.BlockNumber), nameof(S7ServerItem.BlockSize)));
        foreach (var item in S7Servers)
        {
            fileStream.WriteString($"{Environment.NewLine}{string.Join(",", item.AreaKind, item.BlockNumber, item.BlockSize)}");
        }
    }

    private void SaveServerItem()
    {
        SaveServerItem(SavedFileName);
    }

    [RelayCommand]
    private void AddNewItem()
    {
        S7Servers.Add(new());
        CanExportChanged();
    }

    [RelayCommand]
    private void RemoveItem(S7ServerItem item)
    {
        S7Servers.Remove(item);
        CanExportChanged();
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task StartServer()
    {
        var startRes = await _serverService.StartServerAsync(Address, S7Servers.Where(item => item != null).Select(item => item!.ToConfig()));
        IsServerStart = startRes.IsOk;
        if (startRes.IsError)
        {
            MessageHelper.ShowMessage(new MessageContent { Message = startRes.ErrorValue, Icon = Ursa.Controls.MessageBoxIcon.Error });
        }
    }

    [RelayCommand(CanExecute = nameof(CanStop))]
    private async Task StopServer()
    {
        var stopRes = await _serverService.StopServerAsync();
        if (stopRes.IsError)
        {
            MessageHelper.ShowMessage(new MessageContent { Message = stopRes.ErrorValue, Icon = Ursa.Controls.MessageBoxIcon.Error });
        }
        IsServerStart = false;
    }

    [RelayCommand(CanExecute = nameof(CanExport))]
    private async Task ExportServerItems()
    {
        var fileName = await _serviceProvider.GetRequiredService<IStorageProvider>().SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            DefaultExtension = ".csv",
            SuggestedFileName = "db_set.csv",
            ShowOverwritePrompt = true,
            Title = "导出DB设置",
            FileTypeChoices =
            [
                new FilePickerFileType("CSV")
                {
                    Patterns = ["*.csv"],
                    MimeTypes = ["text/csv"],
                }
            ]
        });
        if (fileName != null)
        {
            SaveServerItem(fileName.Path.AbsolutePath);
        }
    }

    [RelayCommand]
    private async Task ImportServerItems()
    {
        var fileNames = await _serviceProvider.GetRequiredService<IStorageProvider>().OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = true,
            Title = "导入DB设置",
            FileTypeFilter =
            [
                new FilePickerFileType("CSV")
                {
                    Patterns = ["*.csv"],
                    MimeTypes = ["text/csv"],
                },
                new FilePickerFileType("ALL")
                {
                    Patterns = ["*.*"],
                    MimeTypes = ["*/*"],
                },
            ]
        });
        if (fileNames != null && fileNames.Count > 0)
        {
            if (MessageBoxResult.Yes == await MessageBox.ShowAsync("是否覆盖当前设置？", "提示", MessageBoxIcon.Question, MessageBoxButton.YesNo))
            {
                ClearItems();
            }
            foreach (var path in fileNames)
            {
                LoadServerItem(path.Path.AbsolutePath);
            }
        }
    }
}

public partial class S7ServerItem : ViewModelBase
{
    [ObservableProperty]
    private AreaKind _areaKind;

    [ObservableProperty]
    private int _blockNumber;

    [ObservableProperty]
    private int _blockSize;

    public AreaConfig ToConfig()
    {
        return new AreaConfig()
        {
            AreaKind = AreaKind,
            BlockNumber = BlockNumber,
            BlockSize = BlockSize
        };
    }
}
