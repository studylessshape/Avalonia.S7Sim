using S7Sim.Services;
using S7Sim.Utils.LogHelper;

namespace PythonRun.Test
{
    internal class ShellCommand : IShellCommand
    {
        public float AcceptInputFloat(string label)
        {
            return new Random().NextSingle() * 100.0f;
        }

        public int AcceptInputInt(string label)
        {
            return new Random().Next(100, 999);
        }

        public string AcceptInputString(string label)
        {
            return "Back for test";
        }

        public void SendLogMessage(string log, int level = 0)
        {
            ConsoLog.Log(log, (LogLevel)level);
        }

        public void ShowMessageBox(string message, int? icon = null)
        {
            ConsoLog.Log(message);
        }
    }
}
