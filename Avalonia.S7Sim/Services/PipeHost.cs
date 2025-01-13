using PipeProtocol;
using S7Sim.Services;

namespace Avalonia.S7Sim.Services
{
    public class PipeHost : PipeBaseHost
    {
        public IShellCommand Shell { get; }
        public PipeHost(IShellCommand shell, IS7DataBlockService dbService, IS7MBService mbService)
        {
            this.Shell = shell;
            RegistCommand("shell", this.Shell);
            RegistCommand("DB", dbService);
            RegistCommand("MB", mbService);
        }

        public override void LogMessage(string message, int level = 0)
        {
            Shell.SendLogMessage(message, level);
        }
    }
}