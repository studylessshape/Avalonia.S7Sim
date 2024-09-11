using Avalonia.S7Sim.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.S7Sim.ViewModels;

public partial class OperationsViewModel : ViewModelBase
{
#if DEBUG
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    public OperationsViewModel()
#pragma warning restore CS8618
    {

    }
#endif

    private readonly IS7DataBlockService _serverService;

    public OperationsViewModel(IS7DataBlockService s7ServerService)
    {
        this._serverService = s7ServerService;
    }

    [ObservableProperty]
    private int _targetDB;

    [ObservableProperty]
    private int _targetPos;

    #region Bit Read and Write
    [ObservableProperty]
    private bool _bitToWrite;

    [ObservableProperty]
    private bool _bitRead;

    [ObservableProperty]
    private byte _targetBitPos;

    [RelayCommand]
    private void WriteBit()
    {
        _serverService.WriteBit(TargetDB, TargetPos, TargetBitPos, BitToWrite);
    }

    [RelayCommand]
    private void ReadBit()
    {
        BitRead = _serverService.ReadBit(TargetDB, TargetPos, TargetBitPos);
    }
    #endregion

    #region Short Read And Write
    [ObservableProperty]
    private short _shortToWrite;

    [ObservableProperty]
    private short _shortRead;

    [RelayCommand]
    private void WriteShort()
    {
        _serverService.WriteShort(TargetDB, TargetPos, ShortToWrite);
    }

    [RelayCommand]
    private void ReadShort()
    {
        ShortRead = _serverService.ReadShort(TargetDB, TargetPos);
    }
    #endregion

    #region UInt Read And Write
    [ObservableProperty]
    private uint _uIntToWrite;

    [ObservableProperty]
    private uint _uIntRead;

    [RelayCommand]
    private void WriteUInt()
    {
        _serverService.WriteUInt32(TargetDB, TargetPos, UIntToWrite);
    }

    [RelayCommand]
    private void ReadUInt()
    {
        UIntRead = _serverService.ReadUInt32(TargetDB, TargetPos);
    }
    #endregion

    #region ULong Read And Write
    [ObservableProperty]
    private ulong _uLongToWrite;

    [ObservableProperty]
    private ulong _uLongRead;

    [RelayCommand]
    private void WriteULong()
    {
        _serverService.WriteULong(TargetDB, TargetPos, ULongToWrite);
    }

    [RelayCommand]
    private void ReadULong()
    {
        ULongRead = _serverService.ReadULong(TargetDB, TargetPos);
    }
    #endregion

    #region Real Read And Write
    [ObservableProperty]
    private float _realToWrite;

    [ObservableProperty]
    private ulong _realRead;

    [RelayCommand]
    private void WriteReal()
    {
        _serverService.WriteReal(TargetDB, TargetPos, RealToWrite);
    }

    [RelayCommand]
    private void ReadReal()
    {
        RealRead = _serverService.ReadULong(TargetDB, TargetPos);
    }
    #endregion

    #region String Read And Write
    [ObservableProperty]
    private string _stringToWrite = "";

    [ObservableProperty]
    private string? _stringRead;

    [ObservableProperty]
    private int _stringMaxLength;

    [RelayCommand]
    private void WriteString()
    {
        _serverService.WriteString(TargetDB, TargetPos, StringMaxLength, StringToWrite);
    }

    [RelayCommand]
    private void ReadString()
    {
        StringRead = _serverService.ReadString(TargetDB, TargetPos);
    }
    #endregion
}
