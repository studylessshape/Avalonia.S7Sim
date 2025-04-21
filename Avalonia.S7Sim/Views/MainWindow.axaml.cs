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
        private readonly IServiceProvider serviceProvider;
        public MainWindow(MainWindowViewModel viewModel, IServiceProvider serviceProvider)
        {
            this.DataContext = viewModel;
            this.serviceProvider = serviceProvider;
            InitializeComponent();

            if (App.Current != null)
            {
                App.Current.RequestedThemeVariant = ThemeVariant.Dark;
            }

            PART_DBConfig.Content = serviceProvider.GetService<ConfigS7ServerView>();
            PART_DBCommand.Content = serviceProvider.GetService<S7CommandView>();
            PART_PyEngine.Content = serviceProvider.GetService<ScriptsView>();
            // PART_LogPanel.Content = serviceProvider.GetService<LogPanel>();
            //PART_DBTable.Content = serviceProvider.GetService<RealtimeView>();
        }

        protected override void OnClosed(EventArgs e)
        {
            var viewModel = serviceProvider.GetService<S7CommandViewModel>();
            viewModel?.Dispose();

            base.OnClosed(e);
        }
    }
}