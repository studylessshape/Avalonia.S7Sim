using Avalonia.S7Sim.Messages;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.MessageHandler;

public class MessageNotificationHandler : INotificationHandler<MessageNotification>
{
    public Task Handle(MessageNotification notification, CancellationToken cancellationToken)
    {
        //MessageBox.Show(notification.Message);
        return Task.CompletedTask;
    }
}