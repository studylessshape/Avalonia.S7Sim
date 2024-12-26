using System.Diagnostics;

namespace Avalonia.S7Sim.Services
{
    public class PipeProfiles
    {
        public string PipeName { get; }
        public PipeProfiles()
        {
            Process thisProcess = Process.GetCurrentProcess();
            PipeName = $"{thisProcess.ProcessName}/{thisProcess.Id}/{thisProcess.MachineName}";
        }
    }
}
