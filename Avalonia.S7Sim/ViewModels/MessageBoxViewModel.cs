using Avalonia.S7Sim.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;
using Ursa.Controls;

namespace Avalonia.S7Sim.ViewModels
{
    public class MessageBoxViewModel : IRecipient<MessageContent>
    {
        public MessageBoxViewModel()
        {
            WeakReferenceMessenger.Default.Register(this);
        }

        public void Receive(MessageContent message)
        {
            Func<string, string?, MessageBoxIcon, MessageBoxButton, Task<MessageBoxResult>> invoke = MessageBox.ShowAsync;

            if (message.Overlay)
            {
                invoke = async (message, title, icon, button) => await MessageBox.ShowOverlayAsync(message, title, icon: icon, button: button);
            }

            if (message.Owner != null && message.Title != null)
            {
                invoke = async (msg, title, icon, button) => await MessageBox.ShowAsync(message.Owner, msg, title!, icon, button);
            }

            try
            {
                invoke.Invoke(message.Message, message.Title, message.Icon, message.Buttons);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
