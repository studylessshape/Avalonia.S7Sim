using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Rendering;
using Avalonia.S7Sim.ViewModels;
using System;

namespace Avalonia.S7Sim.Views;

public partial class RealtimeView : UserControl
{
#if DEBUG
    public RealtimeView()
    {
        InitializeComponent();
    }
#endif

    public RealtimeView(RealtimeViewModel viewModel)
    {
        this.DataContext = viewModel;
        InitializeComponent();
    }
}