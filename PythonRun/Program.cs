using PythonRun;
using S7Sim.Utils.LogHelper;

var envSets = new EnvSets(args);

if (envSets.EnvDirectories.Length == 0)
{
    ConsoLog.LogWarn("Seach Path (-s / --search-paths) is empty!");
}

