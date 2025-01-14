using System.Threading;
using System.Threading.Tasks;

namespace PipeProtocol.Client
{
    public interface IControlCommand
    {
        void Stop();
        Task StopAsync();
    }
}
