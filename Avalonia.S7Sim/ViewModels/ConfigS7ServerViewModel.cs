using Avalonia.S7Sim.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using S7Sim.Services;
using S7Sim.Services.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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

    public ObservableCollection<S7ServerItem?> S7Servers { get; } = [];

    public ConfigS7ServerViewModel(IS7ServerService serverService)
    {
        this._serverService = serverService;
    }

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
