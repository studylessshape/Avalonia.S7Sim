using Avalonia.Controls;
using Avalonia.S7Sim.ViewModels;
using Avalonia.Threading;
using System;
using Ursa.Controls;

namespace Avalonia.S7Sim.Views;


public partial class SubProcessIOWindow : UrsaWindow
{
    public SubProcessIOWindow() : this(new SubProcessIOViewModel())
    {
    }

    public SubProcessIOWindow(SubProcessIOViewModel viewModel)
    {
        InitializeDataContext(viewModel);
        InitializeComponent();
    }

    private void InitializeDataContext(SubProcessIOViewModel viewModel)
    {
        viewModel.CloseWindow += () =>
        {
            try
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    Close();
                });
            }
            catch (Exception)
            {
            }
        };
        viewModel.OnStdOutChangedEvent += ViewModel_OnStdOutChangedEvent;
        viewModel.SetOwnerWindow(this);
        this.DataContext = viewModel;
    }

    private void ViewModel_OnStdOutChangedEvent(string? oldValue, string newValue)
    {
        if (!string.IsNullOrEmpty(oldValue))
        {
            // Because where the method of property chanaged is other thread.
            // When proptery changed and will call `ScrollToEnd` method, it will get error if directly call the method.
            Dispatcher.UIThread.Invoke(() =>
            {
                if (LOG_ScrollViewer.Offset == LOG_ScrollViewer.ScrollBarMaximum)
                {
                    LOG_ScrollViewer.ScrollToEnd();
                }
            });
        }
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        var viewModel = (SubProcessIOViewModel?)this.DataContext;
        if (e.CloseReason == WindowCloseReason.WindowClosing && viewModel?.SubProcess?.HasExited == false && !e.IsProgrammatic)
        {
            e.Cancel = true;
            var result = await MessageBox.ShowAsync(this, "脚本仍在运行中，是否退出？", "警告", icon: MessageBoxIcon.Warning, button: MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
            else
            {
                return;
            }
        }
        base.OnClosing(e);
    }

    protected override void OnClosed(EventArgs e)
    {
        var viewModel = (SubProcessIOViewModel?)this.DataContext;
        viewModel?.Stop(true);

        base.OnClosed(e);
    }
}