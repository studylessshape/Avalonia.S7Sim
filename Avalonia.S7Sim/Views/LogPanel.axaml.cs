using Avalonia.Controls;
using Avalonia.S7Sim.ViewModels;

namespace Avalonia.S7Sim.Views;

public partial class LogPanel : UserControl
{
#if DEBUG
    public LogPanel()
    {
        InitializeComponent();
    }
#endif

    public LogPanel(LogPanelViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();

        viewModel.Messages.CollectionChanged += (_, _) => PART_Scroll.ScrollToEnd();
    }
}