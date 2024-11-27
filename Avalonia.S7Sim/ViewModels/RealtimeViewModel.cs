using Avalonia.Controls;
using Avalonia.S7Sim.Models.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Scripting.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Avalonia.S7Sim.ViewModels;

public partial class RealtimeViewModel : ViewModelBase, IRecipient<UpdateRealtimeOffsetCollectionEvent>
{
    public ObservableCollection<S7DataItem> S7DataItems { get; set; } = new();
    private List<S7DataItem> SelectS7Items { get; set; } = new();
    private bool CanDelete => SelectS7Items.Any();

    [RelayCommand]
    public void AddDataItem()
    {
        S7DataItems.Add(new S7DataItem());
    }

    [RelayCommand]
    private void SelectChanged(SelectionChangedEventArgs eventArgs)
    {
        SelectS7Items.AddRange(eventArgs.AddedItems.Select(o => (S7DataItem)o));
        foreach (var obj in eventArgs.RemovedItems)
        {
            if (obj != null)
            {
                var item = (S7DataItem)obj;
                SelectS7Items.Remove(item);
            }
        }
        OnPropertyChanged(nameof(CanDelete));
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private void RemoveSelects()
    {
        var list = new List<S7DataItem>(SelectS7Items);
        foreach (var item in list)
        {
            S7DataItems.Remove(item);
        }
    }

    public void Receive(UpdateRealtimeOffsetCollectionEvent message)
    {
        throw new System.NotImplementedException();
    }
}

public partial class S7DataItem : ViewModelBase
{
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string dataType;

    [ObservableProperty]
    private int offset;

    [ObservableProperty]
    private byte? bit;

    [ObservableProperty]
    private int? length;

    [ObservableProperty]
    private string? subType;

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

    partial void OnDataTypeChanged(string? oldValue, string newValue)
    {
        if (oldValue != newValue)
        {
            WeakReferenceMessenger.Default.Send(new UpdateRealtimeOffsetCollectionEvent());
        }
    }
}
