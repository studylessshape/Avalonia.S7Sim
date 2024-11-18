namespace Avalonia.S7Sim.Models;

public class DataItem
{
    public DataType DataType { get; set; }
    public int Offsest { get; set; }

    private byte? _bit;
    public byte? Bit
    {
        get => DataType != DataType.Bit ? null : _bit;
        set => _bit = value;
    }

    private int? _length;
    public int? Length
    {
        get => DataType == DataType.String || DataType == DataType.Array ? null : _length;
        set => _length = value;
    }

    private DataType? _subType;
    public DataType? SubType
    {
        get => DataType != DataType.Array ? null : _subType;
        set => _subType = value;
    }
}
