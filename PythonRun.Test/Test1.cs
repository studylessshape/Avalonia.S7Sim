using Avalonia.S7Sim.Services;
using S7Sim.Services.Models;
using System.Net;

namespace PythonRun.Test
{
    [TestClass]
    public sealed class Test1
    {
        const string pipeName = "TestMethodPipe";

        [TestMethod]
        public void TestMethod1()
        {
            EnvSets? envSets = Program.ParseArgs(
            [
                "-n",
                pipeName,
                "-f",
                "D:\\Projects\\rust\\zc_plc_tcp\\task_in2.py"
            ]);

            if (envSets == null)
            {
                return;
            }

            ControlCommand controlCommand = new ControlCommand();

            var scope = Program.CreateScriptEnv(envSets, controlCommand);

            StartPipeHost(controlCommand.StopToken);

            Program.RunScript(envSets.FilePath, scope);

            Avalonia.S7Sim.Services.ControlCommand windowsControl = new Avalonia.S7Sim.Services.ControlCommand($"{pipeName}_py");

            windowsControl.Stop();

            Program.Release(controlCommand);
        }

        void StartPipeHost(CancellationToken stopToken)
        {
            PipeHost pipeHost = new();

            var s7server = new S7ServerService();
            s7server.StartServerAsync(IPAddress.Parse("127.0.0.1"),
            [
                new AreaConfig()
                {
                    AreaKind = AreaKind.DB,
                    BlockNumber = 101,
                    BlockSize = 1000,
                },
                new AreaConfig()
                {
                    AreaKind = AreaKind.DB,
                    BlockNumber = 102,
                    BlockSize = 1000,
                }
            ]);
            var s7dbService = new Avalonia.S7Sim.Services.S7DataBlockService(s7server);
            pipeHost.RegistCommand("shell", new ShellCommand());
            pipeHost.RegistCommand("DB", s7dbService);
            pipeHost.RunAsync(pipeName, stopToken);
        }
    }
}
