namespace Avalonia.S7Sim.Models;

public struct AreaConfig
{
    public AreaKind AreaKind { get; set; }
    public int BlockNumber { get; set; }
    public int BlockSize { get; set; }
}
