using IronPython.Hosting;
using Microsoft.Scripting;
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
            CancellationTokenSource stopTokenSource = new CancellationTokenSource();

            var scope = CreateScriptEnv(envSets, stopTokenSource);
            RunScript(envSets.FilePath, scope);

            Release(stopTokenSource);
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

        public static ScriptScope CreateScriptEnv(EnvSets envSets, CancellationTokenSource stopTokenSource)
        {

            PipeHost pipeHost = new();
            pipeHost.RunAsync($"{envSets.NamedPipe}_py", stopTokenSource.Token);

            IS7DataBlockService dbService = new S7DataBlockService(envSets.NamedPipe, "DB");
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
            scope.SetVariable("shell", shellCommand);
            scope.SetVariable("Logger", ConsoLog.Instance);
            scope.SetVariable("__PY_ENGINE__", engine);

            return scope;
        }

        public static void RunScript(string filePath, ScriptScope scope)
        {
            var source = scope.Engine.CreateScriptSourceFromFile(filePath);
            var code = source.Compile();
            code.Execute(scope);
        }

        public static void Release(CancellationTokenSource source)
        {
            try
            {
                source.Cancel();
            }
            catch (Exception)
            {
            }
        }
    }
}