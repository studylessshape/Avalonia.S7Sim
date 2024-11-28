using System.Collections;
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
    public IList? SelectedItems { get; set; }
    private bool CanDelete => SelectedItems?.Count > 0;

    [RelayCommand]
    public void AddDataItem()
    {
        S7DataItems.Add(new S7DataItem());
    }

    [RelayCommand]
    private void SelectChanged(IList source)
    {
        SelectedItems = source;
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
