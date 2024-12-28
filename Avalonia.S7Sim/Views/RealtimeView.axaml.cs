using Avalonia.Controls;
using Avalonia.S7Sim.ViewModels;

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