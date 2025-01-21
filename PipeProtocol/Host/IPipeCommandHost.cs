using System.Threading;
using System.Threading.Tasks;

namespace PipeProtocol
{
    public interface IPipeCommandHost : IPipeHost
    {
        void RegistCommand(string moduleName, object module);
        void RemoveCommand(string moduleName);
        Task RunOnTaskAsync(string pipeName, CancellationToken stoppingToken = default);
    }
}
