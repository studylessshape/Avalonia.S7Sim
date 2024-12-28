using PipeProtocol;
using S7Sim.Utils.LogHelper;

namespace PythonRun
{
    internal static class LogExtra
    {
        internal static void LogResponse(PipeResponse response)
        {
            if (response.ErrCode == 0)
            {
                ConsoLog.LogInfo($"Receive: {response.Message}");
            }
            else
            {
                ConsoLog.LogError($"Receive Error. Code: {response.ErrCode} ({(ErrCodes)response.ErrCode}){Environment.NewLine}" +
                    $"Message: {response.Message}");
            }
        }
    }
}
