using System.Threading.Tasks;

namespace Avalonia.S7Sim.Services;

public interface IS7ServerService
{
    Task StartServerAsync();
    Task StopServerAsync();
}