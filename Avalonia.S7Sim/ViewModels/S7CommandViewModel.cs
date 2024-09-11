namespace Avalonia.S7Sim.ViewModels;

public partial class S7CommandViewModel : ViewModelBase
{
#if DEBUG
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    public S7CommandViewModel()
#pragma warning restore CS8618
    {
        
    }
#endif

    public S7CommandViewModel(ConfigS7ServerViewModel configModel, OperationsViewModel operationsModel)
    {
        ConfigModel = configModel;
        OperationsViewModel = operationsModel;
    }

    public ConfigS7ServerViewModel ConfigModel { get; }
    public OperationsViewModel OperationsViewModel { get; }
}
