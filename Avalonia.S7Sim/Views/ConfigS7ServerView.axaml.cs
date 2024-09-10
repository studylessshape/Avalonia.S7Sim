using Avalonia.Controls;
using Avalonia.S7Sim.ViewModels;

namespace Avalonia.S7Sim.Views;

public partial class ConfigS7ServerView : UserControl
{
#if DEBUG
    public ConfigS7ServerView()
    {
        InitializeComponent();
    }
#endif

    public ConfigS7ServerView(ConfigS7ServerViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}