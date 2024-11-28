using Avalonia.Controls;
using Avalonia.S7Sim.Messages;
using Avalonia.S7Sim.Views;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using Avalonia.Controls.Notifications;
using Microsoft.Extensions.Logging;
using Ursa.Controls;

namespace Avalonia.S7Sim.Services.Shell;

public class ShellCommand : IShellCommand
{
    private readonly IServiceProvider service;

    public ShellCommand(IServiceProvider service)
    {
        this.service = service;
    }

    protected virtual Window CreateDialogWindow(string label)
    {
        var dialog = new ShellDialogWindow();
        dialog.SetValue(ShellDialogWindow.ButtonsProperty, DialogButton.OK);
        dialog.SetValue(ShellDialogWindow.TitleProperty, label);
        dialog.SetValue(ShellDialogWindow.CanResizeProperty, true);

        dialog.KeyBindings.Add(new Input.KeyBinding()
        {
            Command = new RelayCommand(() =>
            {
                dialog.Close(DialogResult.Cancel);
            }),
            Gesture = new Input.KeyGesture(Input.Key.Escape)
        });

        return dialog;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="label"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    public float AcceptInputFloat(string label)
    {
        return Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var dialog = CreateDialogWindow(label);

            var numberInput = new Avalonia.Controls.NumericUpDown();
            numberInput.SetValue(Avalonia.Controls.NumericUpDown.MinWidthProperty, 200);
            numberInput.SetValue(Avalonia.Controls.NumericUpDown.IncrementProperty, 1);

            dialog.Content = numberInput;
            numberInput.KeyBindings.Add(new Input.KeyBinding()
            {
                Command = new RelayCommand(() =>
                {
                    dialog.Close(DialogResult.OK);
                }),
                Gesture = new Input.KeyGesture(Input.Key.Enter)
            });

            var dialogTask = dialog.ShowDialog<DialogResult?>(service.GetRequiredService<MainWindow>());
            numberInput.Focus();

            var dialogResult = await dialogTask;

            if (dialogResult is null || dialogResult == DialogResult.Cancel || dialogResult == DialogResult.No || dialogResult == DialogResult.None)
            {
                throw new OperationCanceledException();
            }

            return (float)(numberInput.GetValue(Avalonia.Controls.NumericUpDown.ValueProperty) ?? 0);
        }).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="label"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    public int AcceptInputInt(string label)
    {
        return (int)(AcceptInputFloat(label));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="label"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    public string AcceptInputString(string label)
    {
        return Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var dialog = CreateDialogWindow(label);

            var textInput = new TextBox();
            textInput.SetValue(TextBlock.MinWidthProperty, 200);

            dialog.Content = textInput;
            textInput.KeyBindings.Add(new Input.KeyBinding()
            {
                Command = new RelayCommand(() =>
                {
                    dialog.Close(DialogResult.OK);
                }),
                Gesture = new Input.KeyGesture(Input.Key.Enter)
            });

            var dialogTask = dialog.ShowDialog<DialogResult?>(service.GetRequiredService<MainWindow>());
            textInput.Focus();

            var dialogResult = await dialogTask;

            if (dialogResult is null || dialogResult == DialogResult.Cancel || dialogResult == DialogResult.No || dialogResult == DialogResult.None)
            {
                throw new OperationCanceledException();
            }

            return textInput.GetValue(TextBox.TextProperty) ?? "";
        }).GetAwaiter().GetResult();
    }

    public void ShowMessageBox(string message, int? icon = null)
    {
        var content = new MessageContent { Message = message };
        if (icon != null)
        {
            content.Icon = (MessageBoxIcon)icon.Value;
        }

        Dispatcher.UIThread.Invoke(() =>
        {
            MessageHelper.ShowMessage(content);
        });
    }

    public void SendLogMessage(string log, NotificationType level = NotificationType.Information)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            MessageHelper.SendLogMessage(new LogMessage() { Level = level, Message = log });
        });
    }
}
