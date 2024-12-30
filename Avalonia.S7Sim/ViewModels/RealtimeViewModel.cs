using Avalonia.S7Sim.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Scripting.Utils;
using S7Sim.Services;
using S7Sim.Services.Exceptions;
using S7Sim.Services.Models;
using S7Sim.Services.Models.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.ViewModels;

public partial class RealtimeViewModel : ViewModelBase, IRecipient<UpdateRealtimeOffsetEvent>
{
    private readonly IS7DataBlockService _dbService;
    public ObservableCollection<S7DataItem> S7DataItems { get; set; } = new();
    public IList? SelectedItems { get; set; }

    [ObservableProperty] private int? dbNumber;

    private bool CanDelete => SelectedItems?.Count > 0;
    private bool SelectOne => SelectedItems?.Count == 1;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StopScanCommand))]
    private bool _canStopScan;

    private Task? scanTask;
    private CancellationTokenSource? scanTaskTokenSource;

    public RealtimeViewModel(IS7DataBlockService dbService)
    {
        _dbService = dbService;
    }

    [RelayCommand]
    public void AddDataItem()
    {
        S7DataItems.Add(new S7DataItem());
        OnPropertyChanged(nameof(CanDelete));
    }

    [RelayCommand]
    public void InsertDataItem(int index)
    {
        S7DataItems.Insert(index, new S7DataItem());
        OnPropertyChanged(nameof(S7DataItems));
        OnPropertyChanged(nameof(CanDelete));
    }

    [RelayCommand]
    private void SelectChanged(IList source)
    {
        if (SelectedItems != source)
        {
            SelectedItems = source;
        }
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private void RemoveSelects()
    {
        var list = new List<S7DataItem>();
        if (SelectedItems?.Count > 0)
        {
            list.AddRange(SelectedItems.Select(o => (S7DataItem)o));
        }

        foreach (var item in list)
        {
            S7DataItems.Remove(item);
        }

        SelectedItems?.Clear();
        OnPropertyChanged(nameof(S7DataItems));
        OnPropertyChanged(nameof(CanDelete));
    }

    [RelayCommand]
    private async Task StartScan()
    {
        if (scanTask != null)
        {
            if (scanTaskTokenSource != null)
            {
                await scanTaskTokenSource.CancelAsync();
                scanTaskTokenSource = null;
            }
        }

        if (scanTaskTokenSource == null)
        {
            scanTaskTokenSource = new CancellationTokenSource();
        }

        var token = scanTaskTokenSource.Token;
        scanTask = Task.Run(() =>
        {
            var dbService = _dbService;
            var dbNum = this.DbNumber;
            while (!token.IsCancellationRequested)
            {
                if (dbNum == null)
                {
                    continue;
                }

                ScanItems(token, dbService, dbNum.Value);
            }
        }, token);
        CanStopScan = true;
        await scanTask;
    }

    [RelayCommand(CanExecute = nameof(CanStopScan))]
    private void StopScan()
    {
        if (scanTaskTokenSource != null)
        {
            scanTaskTokenSource.Cancel();
            scanTaskTokenSource = null;
            scanTask = null;
        }
        CanStopScan = false;
    }

    private void ScanItems(CancellationToken cancellationToken, IS7DataBlockService dbService, int dbNumber)
    {
        // if (DbNumber == null || DbNumber < 0)
        // {
        //     MessageHelper.SendLogMessage(new LogMessage() { Message = $"DbNumber `{DbNumber}` 不合规，要求至少大于等于 0！"});
        // }

        try
        {
            foreach (var item in S7DataItems)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                bool error = false;

                try
                {
                    item.ReadValue(_dbService, dbNumber);
                }
                catch (IndexOutOfRangeException)
                {
                    MessageHelper.ShowMessage("请求位置超过 DB 块大小");
                    error = true;
                }
                catch (DbNumberNotExistException dbe)
                {
                    MessageHelper.ShowMessage($"DbNumber={dbe.DbNumber} 不存在！");
                    error = true;
                }

                if (error)
                {
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void Receive(UpdateRealtimeOffsetEvent message)
    {
    }
}

public partial class S7DataItem : ViewModelBase
{
    [ObservableProperty] private string name;

    [ObservableProperty] private DataType dataType;

    [ObservableProperty] private int offset;

    [ObservableProperty] private byte? bit;

    [ObservableProperty] private int? length;

    [ObservableProperty] private string? subType;
    [ObservableProperty] private S7DataValue value;

    public S7DataItem()
    {
        Value = new S7DataValue();
        DataType = DataType.Bit;
    }

    partial void OnDataTypeChanged(DataType value)
    {
        Value.DataType = value;
    }

    public void ReadValue(IS7DataBlockService dbService, int dbNumber)
    {
        try
        {
            Value.Value = DataType switch
            {
                DataType.Bit => (Bit == null ? null : dbService.ReadBit(dbNumber, Offset, Bit.Value)),
                DataType.SInt => dbService.ReadByte(dbNumber, Offset),
                DataType.Int => dbService.ReadShort(dbNumber, Offset),
                DataType.DInt => dbService.ReadInt32(dbNumber, Offset),
                DataType.LInt => dbService.ReadLong(dbNumber, Offset),
                DataType.UDInt => dbService.ReadUInt32(dbNumber, Offset),
                DataType.Byte => dbService.ReadByte(dbNumber, Offset),
                DataType.Real => dbService.ReadReal(dbNumber, Offset),
                DataType.LReal => dbService.ReadDouble(dbNumber, Offset),
                _ => null
            };
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

    //public DataItem ToDataItem()
    //{
    //    return new DataItem
    //    {
    //        DataType = DataType,
    //        Offsest = Offset,
    //        Bit = Bit,
    //        Length = Length,
    //        SubType = SubType
    //    };
    //}

    partial void OnDataTypeChanged(DataType oldValue, DataType newValue)
    {
        if (oldValue != newValue)
        {
            WeakReferenceMessenger.Default.Send(new UpdateRealtimeOffsetEvent());
        }
    }
}

public partial class S7DataValue : ViewModelBase
{
    private const string ValueTypeError = "ERROR: 值类型与指定的数据类型不匹配";
    private const string ValueTypeUnsupported = "ERROR: 不支持的数据类型";

    [ObservableProperty] private DataType _dataType;

    [ObservableProperty] private object? _value;

    private string ValueToType<T>(object? value)
    {
        switch (value)
        {
            case null:
                return "null";
            case T val:
                {
                    var to = val.ToString();
                    return to ?? "null";
                }
            default:
                return ValueTypeError;
        }
    }

    public string Display
    {
        get
        {
            switch (DataType)
            {
                case DataType.Bit:
                    return ValueToType<bool>(Value);
                case DataType.SInt:
                    return ValueToType<byte>(Value);
                case DataType.Int:
                    return ValueToType<short>(Value);
                case DataType.DInt:
                    return ValueToType<int>(Value);
                case DataType.LInt:
                    return ValueToType<long>(Value);
                case DataType.UDInt:
                    return ValueToType<uint>(Value);
                case DataType.Byte:
                    return ValueToType<byte>(Value);
                // case DataType.Word:
                //     return ValueToType<ushort>(Value);
                // case DataType.DWord:
                //     return ValueToType<uint>(Value);
                case DataType.Real:
                    return ValueToType<float>(Value);
                case DataType.LReal:
                    return ValueToType<double>(Value);
                default:
                    return ValueTypeUnsupported;
            }
        }
    }
}