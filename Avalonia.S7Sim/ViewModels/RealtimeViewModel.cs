using Avalonia.S7Sim.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Avalonia.S7Sim.ViewModels;

public partial class RealtimeViewModel : ViewModelBase
{
    public ObservableCollection<S7DataItem> S7DataItems { get; set; } = new();

    [RelayCommand]
    public void AddDataItem()
    {
        S7DataItems.Add(new S7DataItem());
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
}
