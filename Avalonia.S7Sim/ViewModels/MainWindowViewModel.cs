using Avalonia.S7Sim.Messages;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Avalonia.S7Sim.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [RelayCommand]
        private void AddLog()
        {
            WeakReferenceMessenger.Default.Send(new LogMessage()
            {
                Level = Controls.Notifications.NotificationType.Information,
                Message = "test message"
            });
        }
    }
}
