using Avalonia.Controls.Notifications;
using System;

namespace Avalonia.S7Sim.Messages;

public class LogMessage
{
    public NotificationType Level { get; set; }
    public string? Message { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.Now;
}
