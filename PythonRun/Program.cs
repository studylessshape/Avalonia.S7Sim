using IronPython.Hosting;
using PythonRun;
using S7Sim.Services;
using S7Sim.Utils.LogHelper;

EnvSets envSets;

try
{
    envSets = new EnvSets(args);
}
catch (Exception e)
{
    ConsoLog.LogError($"{e.Message}");
    return;
}

if (envSets.EnvDirectories.Length == 0)
{
    ConsoLog.LogWarn("Seach Path (-s / --search-paths) is empty!");
}

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

var source = engine.CreateScriptSourceFromFile(envSets.FilePath);
var code = source.Compile();

code.Execute(scope);