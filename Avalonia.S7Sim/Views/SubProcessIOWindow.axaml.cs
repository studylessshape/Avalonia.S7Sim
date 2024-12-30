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
        //this.LOG_ScrollViewer.AddHandler(ScrollViewer.ScrollChangedEvent, ScrollViewer_ScrollChanged);
        //this.LOG_ScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
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
                LOG_ScrollViewer.ScrollToEnd();
            });
        }
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        var viewModel = (SubProcessIOViewModel?)this.DataContext;
        if (e.CloseReason == WindowCloseReason.WindowClosing && viewModel?.SubProcess?.HasExited == false && !e.IsProgrammatic)
        {
            e.Cancel = true;
            var result = await MessageBox.ShowAsync(this, "�ű����������У��Ƿ��˳���", "����", icon: MessageBoxIcon.Warning, button: MessageBoxButton.YesNo);
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