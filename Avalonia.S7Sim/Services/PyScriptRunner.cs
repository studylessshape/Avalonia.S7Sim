using Avalonia.S7Sim.Services.Shell;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace Avalonia.S7Sim.Services;

public class PyScriptRunner
{
    private readonly IS7DataBlockService _plcDB;
    private readonly IS7MBService _plcMB;
    private readonly IS7ServerService _server;
    private readonly IShellCommand _shellCommand;

    public ScriptEngine PyEngine { get; }
    private ScriptScope? pyScope = null;

    public PyScriptRunner(IS7DataBlockService plcDB, IS7MBService plcMB, IS7ServerService plcServer, IShellCommand shellCommand)
    {
        this._plcDB = plcDB;
        this._plcMB = plcMB;
        this._server = plcServer;
        this._shellCommand = shellCommand;
        PyEngine = Python.CreateEngine();
    }

    public void RunFile(string filePath)
    {
        if (pyScope is null)
        {
            pyScope = PyEngine.CreateScope();
            pyScope.SetVariable("s7_server_svc", this._plcDB);
            pyScope.SetVariable("S7", this._plcDB);

            pyScope.SetVariable("Server", this._server);
            pyScope.SetVariable("DB", this._plcDB);
            pyScope.SetVariable("MB", this._plcMB);
            pyScope.SetVariable("shell", _shellCommand);

            pyScope.SetVariable("__PY_ENGINE__", this.PyEngine);
        }

        var source = PyEngine.CreateScriptSourceFromFile(filePath);
        var code = source.Compile();

        code.Execute(pyScope);
    }
}
