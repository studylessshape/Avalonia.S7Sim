using Avalonia.Controls;
using Avalonia.S7Sim.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia.S7Sim.Views;

public partial class ConfigS7ServerView : UserControl
{
    public ConfigS7ServerView()
    {
        DataContext = App.AppCurrent?.ServiceProvider.GetService<ConfigS7ServerViewModel>();
        InitializeComponent();
    }
}