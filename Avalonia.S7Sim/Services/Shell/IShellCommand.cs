using Avalonia.Controls.Notifications;

namespace Avalonia.S7Sim.Services.Shell
{
    public interface IShellCommand
    {
        string AcceptInputString(string label);
        int AcceptInputInt(string label);
        float AcceptInputFloat(string label);
        void ShowMessageBox(string message, int? icon = null);
        void SendLogMessage(string log, NotificationType level = NotificationType.Information);
    }
}
