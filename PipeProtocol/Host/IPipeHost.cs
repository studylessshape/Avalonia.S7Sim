using System.Threading;

namespace PipeProtocol
{
    public interface IPipeHost
    {
        string PipeName { get; }
        void RunAsync(string pipeName, CancellationToken token = default);
    }
}
