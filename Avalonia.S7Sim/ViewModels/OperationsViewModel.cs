using Avalonia.S7Sim.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using S7Sim.Services;
using S7Sim.Services.Exceptions;
using System;

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

    [ObservableProperty] private int _targetDB;

    [ObservableProperty] private int _targetPos;

    #region Bit Read and Write

    [ObservableProperty] private bool _bitToWrite;

    [ObservableProperty] private bool _bitRead;

    [ObservableProperty] private byte _targetBitPos;

    [RelayCommand]
    private void WriteBit()
    {
        try
        {
            _serverService.WriteBit(TargetDB, TargetPos, TargetBitPos, BitToWrite);
        }
        catch (IndexOutOfRangeException)
        {
            MessageHelper.ShowMessage("请求位置超过 DB 块大小");
        }
        catch (DbNumberNotExistException dbe)
        {
            MessageHelper.ShowMessage($"DbNumber={dbe.DbNumber} 不存在！");
        }
    }

    [RelayCommand]
    private void ReadBit()
    {
        try
        {
            BitRead = _serverService.ReadBit(TargetDB, TargetPos, TargetBitPos);
        }
        catch (IndexOutOfRangeException)
        {
            MessageHelper.ShowMessage("请求位置超过 DB 块大小");
        }
        catch (DbNumberNotExistException dbe)
        {
            MessageHelper.ShowMessage($"DbNumber={dbe.DbNumber} 不存在！");
        }
    }

    #endregion

    #region Short Read And Write

    [ObservableProperty] private short _shortToWrite;

    [ObservableProperty] private short _shortRead;

    [RelayCommand]
    private void WriteShort()
    {
        try
        {
            _serverService.WriteShort(TargetDB, TargetPos, ShortToWrite);
        }
        catch (IndexOutOfRangeException)
        {
            MessageHelper.ShowMessage("请求位置超过 DB 块大小");
        }
        catch (DbNumberNotExistException dbe)
        {
            MessageHelper.ShowMessage($"DbNumber={dbe.DbNumber} 不存在！");
        }
    }

    [RelayCommand]
    private void ReadShort()
    {
        try
        {
            ShortRead = _serverService.ReadShort(TargetDB, TargetPos);
        }
        catch (IndexOutOfRangeException)
        {
            MessageHelper.ShowMessage("请求位置超过 DB 块大小");
        }
        catch (DbNumberNotExistException dbe)
        {
            MessageHelper.ShowMessage($"DbNumber={dbe.DbNumber} 不存在！");
        }
    }

    #endregion

    #region UInt Read And Write

    [ObservableProperty] private uint _uIntToWrite;

    [ObservableProperty] private uint _uIntRead;

    [RelayCommand]
    private void WriteUInt()
    {
        try
        {
            _serverService.WriteUInt32(TargetDB, TargetPos, UIntToWrite);
        }
        catch (IndexOutOfRangeException)
        {
            MessageHelper.ShowMessage("请求位置超过 DB 块大小");
        }
        catch (DbNumberNotExistException dbe)
        {
            MessageHelper.ShowMessage($"DbNumber={dbe.DbNumber} 不存在！");
        }
    }

    [RelayCommand]
    private void ReadUInt()
    {
        try
        {
            UIntRead = _serverService.ReadUInt32(TargetDB, TargetPos);
        }
        catch (IndexOutOfRangeException)
        {
            MessageHelper.ShowMessage("请求位置超过 DB 块大小");
        }
        catch (DbNumberNotExistException dbe)
        {
            MessageHelper.ShowMessage($"DbNumber={dbe.DbNumber} 不存在！");
        }
    }

    #endregion

    #region ULong Read And Write

    [ObservableProperty] private ulong _uLongToWrite;

    [ObservableProperty] private ulong _uLongRead;

    [RelayCommand]
    private void WriteULong()
    {
        try
        {
            _serverService.WriteULong(TargetDB, TargetPos, ULongToWrite);
        }
        catch (IndexOutOfRangeException)
        {
            MessageHelper.ShowMessage("请求位置超过 DB 块大小");
        }
        catch (DbNumberNotExistException dbe)
        {
            MessageHelper.ShowMessage($"DbNumber={dbe.DbNumber} 不存在！");
        }
    }

    [RelayCommand]
    private void ReadULong()
    {
        try
        {
            ULongRead = _serverService.ReadULong(TargetDB, TargetPos);
        }
        catch (IndexOutOfRangeException)
        {
            MessageHelper.ShowMessage("请求位置超过 DB 块大小");
        }
        catch (DbNumberNotExistException dbe)
        {
            MessageHelper.ShowMessage($"DbNumber={dbe.DbNumber} 不存在！");
        }
    }

    #endregion

    #region Real Read And Write

    [ObservableProperty] private float _realToWrite;

    [ObservableProperty] private ulong _realRead;

    [RelayCommand]
    private void WriteReal()
    {
        try
        {
            _serverService.WriteReal(TargetDB, TargetPos, RealToWrite);
        }
        catch (IndexOutOfRangeException)
        {
            MessageHelper.ShowMessage("请求位置超过 DB 块大小");
        }
        catch (DbNumberNotExistException dbe)
        {
            MessageHelper.ShowMessage($"DbNumber={dbe.DbNumber} 不存在！");
        }
    }

    [RelayCommand]
    private void ReadReal()
    {
        try
        {
            RealRead = _serverService.ReadULong(TargetDB, TargetPos);
        }
        catch (IndexOutOfRangeException)
        {
            MessageHelper.ShowMessage("请求位置超过 DB 块大小");
        }
        catch (DbNumberNotExistException dbe)
        {
            MessageHelper.ShowMessage($"DbNumber={dbe.DbNumber} 不存在！");
        }
    }

    #endregion

    #region String Read And Write

    [ObservableProperty] private string _stringToWrite = "";

    [ObservableProperty] private string? _stringRead;

    [ObservableProperty] private int _stringMaxLength = 256;

    [RelayCommand]
    private void WriteString()
    {
        try
        {
            _serverService.WriteString(TargetDB, TargetPos, StringMaxLength, StringToWrite);
        }
        catch (IndexOutOfRangeException)
        {
            MessageHelper.ShowMessage("请求位置超过 DB 块大小");
        }
        catch (DbNumberNotExistException dbe)
        {
            MessageHelper.ShowMessage($"DbNumber={dbe.DbNumber} 不存在！");
        }
    }

    [RelayCommand]
    private void ReadString()
    {
        try
        {
            StringRead = _serverService.ReadString(TargetDB, TargetPos);
        }
        catch (IndexOutOfRangeException)
        {
            MessageHelper.ShowMessage("请求位置超过 DB 块大小");
        }
        catch (DbNumberNotExistException dbe)
        {
            MessageHelper.ShowMessage($"DbNumber={dbe.DbNumber} 不存在！");
        }
    }
    #endregion
}