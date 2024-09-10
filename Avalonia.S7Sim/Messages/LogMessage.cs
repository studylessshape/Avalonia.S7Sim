using Microsoft.Extensions.Logging;
using System;

namespace Avalonia.S7Sim.Messages;

public class LogMessage
{
    public LogLevel Level { get; set; }
    public string? Message { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.Now;
}
