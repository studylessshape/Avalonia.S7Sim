using S7Sim.Services.Models;
using S7Sim.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace S7Sim.Services;

public interface IS7ServerService
{
    IList<RunningServerItem> RunningItems { get; }
    Task<Result<ValueTuple, string>> StartServerAsync(IPAddress? address, IEnumerable<AreaConfig> areaConfigss);
    Task<Result<ValueTuple, string>> StopServerAsync();
}