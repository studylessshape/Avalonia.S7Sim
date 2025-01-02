using PipeProtocol;
using S7Sim.Utils.LogHelper;

namespace PythonRun
{
    internal static class ProtocolExtra
    {
        internal static PipeResponse SendCommand(string pipeName, string module, string methodName, params string[] parameters)
        {
            ConsoLog.LogInfo($"Send command: {module}.{methodName} {parameters?.AsString(",")}");
            var response = ProtocolTools.SendCommand(pipeName, module, methodName, parameters);
            if (!(response.ErrCode == (int)ErrCodes.None && response.Message.Length == 0))
            {
                LogExtra.LogResponse(response);
            }

            if (response.ErrCode != (int)ErrCodes.None)
            {
                throw new Exception(response.Message);
            }

            return response;
        }
    }
}
