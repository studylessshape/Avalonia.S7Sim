using Avalonia.Controls;
using Avalonia.S7Sim.ViewModels;

namespace Avalonia.S7Sim.Views;

public partial class S7CommandView : UserControl
{
#if DEBUG
    public S7CommandView()
    {
        InitializeComponent();
    }
#endif

    public S7CommandView(S7CommandViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}