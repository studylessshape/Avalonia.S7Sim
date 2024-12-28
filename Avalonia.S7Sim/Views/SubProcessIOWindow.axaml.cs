using Avalonia.Controls;
using Avalonia.S7Sim.ViewModels;
using Avalonia.Threading;
using System;

namespace Avalonia.S7Sim.Views;

public partial class SubProcessIOWindow : Window
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

    protected override void OnClosed(EventArgs e)
    {
        var viewModel = (SubProcessIOViewModel?)this.DataContext;
        viewModel?.Stop(true);

        base.OnClosed(e);
    }
}