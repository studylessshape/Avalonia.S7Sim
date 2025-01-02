namespace Avalonia.S7Sim.Services
{
    public interface IPipeCommandHost : IPipeHost
    {
        void RegistCommand(string moduleName, object module);
        void RemoveCommand(string moduleName);
    }
}
