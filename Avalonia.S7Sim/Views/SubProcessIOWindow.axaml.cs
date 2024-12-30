using Avalonia.Controls;
using Avalonia.S7Sim.ViewModels;
using Avalonia.Threading;
using System;
using Ursa.Controls;

namespace Avalonia.S7Sim.Views;

public partial class SubProcessIOWindow : UrsaWindow
{
    public SubProcessIOWindow()
    {
        InitializeDataContext(new());
        InitializeComponent();
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
            Dispatcher.UIThread.Invoke(() =>
            {
                Close();
            });
        };
        this.DataContext = viewModel;
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        var viewModel = (SubProcessIOViewModel?)this.DataContext;
        if (e.CloseReason == WindowCloseReason.WindowClosing && viewModel?.SubProcess?.HasExited == false && !e.IsProgrammatic)
        {
            e.Cancel = true;
            //var result = await MessageBox.ShowAsync(owner: (Window)this, "脚本仍在运行中，是否退出？", icon: MessageBoxIcon.Warning, button: MessageBoxButton.YesNo);
            var result = await MessageBox.ShowAsync(this, "脚本仍在运行中，是否退出？", "警告", icon: MessageBoxIcon.Warning, button: MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Close();
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