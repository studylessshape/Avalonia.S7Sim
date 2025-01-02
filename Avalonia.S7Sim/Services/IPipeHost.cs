using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.Services
{
    public interface IPipeHost
    {
        string? PipeName { get; }
        void RunAsync(string pipeName, CancellationToken token = default);
    }
}
