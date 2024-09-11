using CommunityToolkit.Mvvm.Messaging;

namespace Avalonia.S7Sim.Messages;

public static class MessageHelper
{
    public static void ShowMessage(MessageContent content)
    {
        WeakReferenceMessenger.Default.Send(content);
    }

    public static void SendLogMessage(LogMessage msg)
    {
        WeakReferenceMessenger.Default.Send(msg);
    }
}
