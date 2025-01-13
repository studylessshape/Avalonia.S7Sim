using PipeProtocol;
using S7Sim.Utils.LogHelper;

namespace PythonRun.Test
{
    internal class PipeHost : PipeBaseHost
    {
        public override void LogMessage(string message, int level = 0)
        {
            ConsoLog.Log(message, (LogLevel)level);
        }
    }
}
