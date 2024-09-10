using MediatR;

namespace Avalonia.S7Sim.Messages;

public class LogMessageNotification : INotification
{
    public LogMessage Message { get; set; }
}
