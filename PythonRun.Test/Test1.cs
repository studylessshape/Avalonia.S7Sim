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
        public async Task TestMethod1()
        {
            EnvSets? envSets = Program.ParseArgs(
            [
                "-n",
                pipeName,
                "-f",
                "times.py"
            ]);

            if (envSets == null)
            {
                return;
            }

            ControlCommand controlCommand = new();
            CancellationTokenSource stopTokenSource = new CancellationTokenSource();

            var scope = Program.CreateScriptEnv(envSets, controlCommand, stopTokenSource.Token);

            StartPipeHost(controlCommand.StopToken);

            var task = Task.Run(() => Program.RunScript(envSets.FilePath, scope));

            Avalonia.S7Sim.Services.ControlCommand windowsControl = new Avalonia.S7Sim.Services.ControlCommand($"{pipeName}_py");

            windowsControl.Stop();

            await task;

            Program.Release(controlCommand, stopTokenSource);
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
