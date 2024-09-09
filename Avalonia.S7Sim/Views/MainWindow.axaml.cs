using Avalonia.Controls;
using Avalonia.S7Sim.Services.Dialog;
using Avalonia.S7Sim.ViewModels;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.Views
{
    public partial class MainWindow : Window, IDialogBox
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

        public DialogResult ShowDialog(string message, string? title = null, DialogButton dialogButton = DialogButton.Close)
        {
            throw new System.NotImplementedException();
        }

        public Task<DialogResult> ShowDialogAsync(string message, string? title = null, DialogButton dialogButton = DialogButton.Close)
        {
            throw new System.NotImplementedException();
        }
    }
}