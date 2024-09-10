using Avalonia.S7Sim.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using Ursa.Controls;

namespace Avalonia.S7Sim.Views
{
    public partial class MainWindow : UrsaWindow
    {
#if DEBUG
        public MainWindow()
        {
            InitializeComponent();
        }
#endif

        public MainWindow(MainWindowViewModel viewModel, IServiceProvider serviceProvider)
        {
            this.DataContext = viewModel;
            InitializeComponent();

            PART_DBConfig.Content = serviceProvider.GetService<ConfigS7ServerView>();
        }
    }
}