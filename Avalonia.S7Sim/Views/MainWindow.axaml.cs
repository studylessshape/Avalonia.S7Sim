using Avalonia.S7Sim.ViewModels;
using Avalonia.Styling;
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

            if (App.Current != null)
            {
                App.Current.RequestedThemeVariant = ThemeVariant.Dark;
            }

            PART_DBConfig.Content = serviceProvider.GetService<ConfigS7ServerView>();
            PART_LogPanel.Content = serviceProvider.GetService<LogPanel>();
            PART_DBCommand.Content = serviceProvider.GetService<S7CommandView>();
        }
    }
}