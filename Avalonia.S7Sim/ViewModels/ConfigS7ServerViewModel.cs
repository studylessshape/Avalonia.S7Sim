using Avalonia.S7Sim.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.ViewModels;

public partial class ConfigS7ServerViewModel : ViewModelBase
{
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

    public ObservableCollection<S7ServerItem> S7Servers { get; } = new()
    {
        new S7ServerItem()
        {
            AreaKind = AreaKind.DB,
            BlockNumber = 1,
            BlockSize = 1
        },
    };

    [RelayCommand]
    private void AddNewItem()
    {
        S7Servers.Add(new());
    }

    [RelayCommand]
    private void RemoveItem(S7ServerItem item)
    {
        S7Servers.Remove(item);
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task StartServer()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));

        IsServerStart = true;
    }

    [RelayCommand(CanExecute = nameof(CanStop))]
    private void StopServer()
    {
        IsServerStart = false;
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
