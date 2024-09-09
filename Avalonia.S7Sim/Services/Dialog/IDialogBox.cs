using System.Threading.Tasks;

namespace Avalonia.S7Sim.Services.Dialog;

public interface IDialogBox
{
    DialogResult ShowDialog(string message, string? title = null, DialogButton dialogButton = DialogButton.OK | DialogButton.Close);
    Task<DialogResult> ShowDialogAsync(string message, string? title = null, DialogButton dialogButton = DialogButton.OK | DialogButton.Close);
}
