using Avalonia.Controls;
using Avalonia.S7Sim.ViewModels;

namespace Avalonia.S7Sim.Views;

public partial class ScriptsView : UserControl
{
#if DEBUG
    public ScriptsView()
    {
        InitializeComponent();
    }
#endif

    public ScriptsView(ScriptsViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}