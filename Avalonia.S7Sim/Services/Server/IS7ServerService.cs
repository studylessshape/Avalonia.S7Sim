using Avalonia.S7Sim.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.Services;

public interface IS7ServerService
{
    IList<RunningServerItem> RunningItems { get; }
    Task<bool> StartServerAsync(IPAddress? address, IEnumerable<AreaConfig> areaConfigss);
    Task<bool> StopServerAsync();
}