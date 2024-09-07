using Avalonia.Controls;
using Avalonia.S7Sim.ViewModels;

namespace Avalonia.S7Sim.Views
{
    public partial class MainWindow : Window
    {
#if DEBUG
        public MainWindow()
        {
            InitializeComponent();
        }
#endif

        public MainWindow(MainWindowViewModel viewModel)
        {
            this.DataContext = viewModel;

            InitializeComponent();
        }
    }
}