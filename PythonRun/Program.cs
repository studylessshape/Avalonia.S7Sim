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

