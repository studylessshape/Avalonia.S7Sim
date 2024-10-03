using Avalonia.Controls;
using Avalonia.S7Sim.ViewModels;

namespace Avalonia.S7Sim.Views;

public partial class PyEngineView : UserControl
{
#if DEBUG
    public PyEngineView()
    {
        InitializeComponent();
    }
#endif

    public PyEngineView(PyEngineViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}