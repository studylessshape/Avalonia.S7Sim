﻿using S7Sim.Services;

namespace PythonRun
{
    internal class ShellCommand : IShellCommand
    {
        private readonly string pipeName;

        public ShellCommand(string pipeName, string module)
        {
            this.pipeName = pipeName;
            Module = module;
        }

        public string Module { get; }

        public float AcceptInputFloat(string label)
        {
            var response = ProtocolExtra.SendCommand(pipeName, Module, nameof(AcceptInputFloat), [$"\"{label}\""]);
            return float.Parse(response.Message);
        }

        public int AcceptInputInt(string label)
        {
            var response = ProtocolExtra.SendCommand(pipeName, Module, nameof(AcceptInputInt), [$"\"{label}\""]);
            return int.Parse(response.Message);
        }

        public string AcceptInputString(string label)
        {
            var response = ProtocolExtra.SendCommand(pipeName, Module, nameof(AcceptInputString), [$"\"{label}\""]);
            return response.Message;
        }

        public void SendLogMessage(string log, int level = 0)
        {
            ProtocolExtra.SendCommand(pipeName, Module, nameof(SendLogMessage), [$"\"{log}\"", level.ToString()]);
        }

        public void ShowMessageBox(string message, int? icon = null)
        {
            ProtocolExtra.SendCommand(pipeName, Module, nameof(ShowMessageBox), [$"\"{message}\"", icon?.ToString() ?? "null"]);
        }
    }
}
