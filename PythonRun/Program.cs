using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using S7Sim.Services;
using S7Sim.Utils.LogHelper;

namespace PythonRun
{
    public class Program
    {
        private static void Main(string[] args)
        {
            EnvSets? envSets = ParseArgs(args);

            if (envSets == null)
            {
                return;
            }

            ControlCommand controlCommand = new ControlCommand();
            CancellationTokenSource stopTokenSource = new CancellationTokenSource();
            var scope = CreateScriptEnv(envSets, controlCommand, stopTokenSource.Token);
            RunScript(envSets.FilePath, scope);

            Release(controlCommand, stopTokenSource);
        }

        public static EnvSets? ParseArgs(string[] args)
        {
            EnvSets envSets;

            try
            {
                envSets = new EnvSets(args);
            }
            catch (Exception e)
            {
                ConsoLog.LogError($"{e.Message}");
                return null;
            }

            if (envSets.EnvDirectories.Length == 0)
            {
                ConsoLog.LogWarn("Seach Path (-s / --search-paths) is empty!");
            }

            return envSets;
        }

        public static ScriptScope CreateScriptEnv(EnvSets envSets, ControlCommand controlCommand, CancellationToken pipeStopToken)
        {
            var stopToken = controlCommand.StopToken;

            PipeHost pipeHost = new();
            pipeHost.RegistCommand("control", controlCommand);
            pipeHost.RunAsync($"{envSets.NamedPipe}_py", pipeStopToken);

            IS7DataBlockService dbService = new S7DataBlockService(envSets.NamedPipe, "DB");
            IS7MBService mbService = new S7MBService(envSets.NamedPipe, "MB");
            IShellCommand shellCommand = new ShellCommand(envSets.NamedPipe, "shell");

            var engine = Python.CreateEngine();

            List<string> searchPaths = [.. engine.GetSearchPaths()];
            var processPath = Path.GetDirectoryName(Environment.ProcessPath);
            if (!string.IsNullOrEmpty(processPath))
            {
                searchPaths.Add(Path.Combine(processPath, "predefined/s7svrsim"));
            }

            searchPaths.AddRange(envSets.EnvDirectories);
            engine.SetSearchPaths(searchPaths);

            var scope = engine.CreateScope();

            scope.SetVariable("DB", dbService);
            scope.SetVariable("S7", dbService);
            scope.SetVariable("MB", mbService);
            scope.SetVariable("shell", shellCommand);
            scope.SetVariable("Logger", ConsoLog.Instance);
            scope.SetVariable("__PY_ENGINE__", engine);
            scope.SetVariable("ct", stopToken);

            return scope;
        }

        public static void RunScript(string filePath, ScriptScope scope)
        {
            var source = scope.Engine.CreateScriptSourceFromFile(filePath);
            var code = source.Compile();
            code.Execute(scope);
        }

        public static void Release(ControlCommand controlCommand, CancellationTokenSource stopTokenSource)
        {
            try
            {
                controlCommand.Stop();
            }
            catch (Exception)
            {
            }

            try
            {
                stopTokenSource.Cancel();
            }
            catch (Exception)
            {

            }
        }
    }
}