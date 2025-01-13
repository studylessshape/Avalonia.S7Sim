using System.Threading.Tasks;

namespace PipeProtocol
{
    public interface IPipeCommandHost : IPipeHost
    {
        void RegistCommand(string moduleName, object module);
        void RemoveCommand(string moduleName);
    }
}
