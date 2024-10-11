using Avalonia.Interactivity;
using Irihi.Avalonia.Shared.Contracts;
using Ursa.Controls;

namespace Avalonia.S7Sim.Services.Shell;

public class ShellDialogWindow : DefaultDialogWindow
{
    protected override void OnCloseButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (DataContext is IDialogContext context)
        {
            context.Close();
        }
        else
        {
            DialogResult? result = Buttons switch
            {
                DialogButton.None => DialogResult.None,
                DialogButton.OK => null,
                DialogButton.OKCancel => DialogResult.Cancel,
                DialogButton.YesNo => DialogResult.No,
                DialogButton.YesNoCancel => DialogResult.Cancel,
                _ => DialogResult.None
            };
            Close(result);
        }
    }
}
