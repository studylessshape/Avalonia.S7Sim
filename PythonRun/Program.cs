using IronPython.Hosting;
using PythonRun;
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

var engine = Python.CreateEngine();

List<string> searchPaths = engine.GetSearchPaths().ToList();
searchPaths.AddRange(envSets.EnvDirectories);
engine.SetSearchPaths(searchPaths);

var scope = engine.CreateScope();
var source = engine.CreateScriptSourceFromFile(envSets.FilePath);
var code = source.Compile();

code.Execute(scope);