namespace Avalonia.S7Sim.Models;

public class RunningServerItem
{
    public AreaKind AreaKind { get; set; }

    public int BlockNumber { get; set; }

    public int BlockSize { get; set; }

    public byte[] Bytes { get; set; }
}
