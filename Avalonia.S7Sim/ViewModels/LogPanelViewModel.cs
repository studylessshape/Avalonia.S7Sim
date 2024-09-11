using Avalonia.S7Sim.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace Avalonia.S7Sim.ViewModels
{
    public partial class LogPanelViewModel : ViewModelBase, IRecipient<LogMessage>
    {
        public ObservableCollection<LogMessage> Messages { get; } = new();

        public LogPanelViewModel()
        {
            WeakReferenceMessenger.Default.Register(this);
        }

        public void Receive(LogMessage message)
        {
            Messages.Add(message);
        }
    }
}
