using Avalonia.Controls.Notifications;
using PipeProtocol;
using PipeProtocol.Client;
using S7Sim.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.Services
{
    public class ControlCommand : IControlCommand
    {
        private readonly IShellCommand? shellCommand;

        public string PipeName { get; private set; }

        public ControlCommand(string pipeName)
        {
            PipeName = pipeName;
        }

        public ControlCommand(string pipeName, IShellCommand shellCommand) : this(pipeName)
        {
            this.shellCommand = shellCommand;
        }

        public void Stop()
        {
            var response =  ProtocolTools.SendCommand(PipeName, "control", nameof(Stop));
            if (response.ErrCode != 0)
            {
                shellCommand?.SendLogMessage(response.Message, (int)NotificationType.Error);
            }
        }

        public async Task StopAsync()
        {
            var response = await ProtocolTools.SendCommandAsync(PipeName, "control", nameof(Stop), null);
            if (response.ErrCode != 0)
            {
                shellCommand?.SendLogMessage(response.Message, (int)NotificationType.Error);
            }
        }

        public async Task StopAsync(CancellationToken stopToken)
        {
            var response = await ProtocolTools.SendCommandAsync(PipeName, "control", nameof(Stop), null, stopToken);
            if (response.ErrCode != 0)
            {
                shellCommand?.SendLogMessage(response.Message, (int)NotificationType.Error);
            }
        }
    }
}
