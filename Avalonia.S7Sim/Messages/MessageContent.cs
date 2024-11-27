using Avalonia.Controls;
using Ursa.Controls;

namespace Avalonia.S7Sim.Messages;

public class MessageContent
{
    public required string Message { get; set; }
    public string? Title { get; set; }
    public MessageBoxIcon Icon { get; set; } = MessageBoxIcon.None;
    public MessageBoxButton Buttons { get; set; } = MessageBoxButton.OK;
    public string? StyleClass { get; set; }
    public Window? Owner { get; set; }
    public bool Overlay { get; set; } = true;

    public static implicit operator MessageContent(string message)
    {
        return new MessageContent { Message = message };
    }
}
