using Avalonia.S7Sim.Models;

namespace Avalonia.S7Sim.ViewModels;

public class ServerItemViewModel
{
    public AreaKind AreaKind { get; set; }

    public int BlockNumber { get; set; }

    public int BlockSize { get; set; }
}
