using MediatR;

namespace Avalonia.S7Sim.Messages;

public class MessageNotification : INotification
{
    public required string Message { get; set; }
}
