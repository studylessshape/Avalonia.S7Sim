using Avalonia.S7Sim.Services.Shell;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using S7Sim.Services.DB;
using S7Sim.Services.MB;
using S7Sim.Services.Scripts;
using S7Sim.Services.Server;

namespace Avalonia.S7Sim.Services;

public class PyScriptRunner : ScriptRunner
{
    private readonly IS7DataBlockService _plcDB;
    private readonly IS7MBService _plcMB;
    private readonly IS7ServerService _server;
    private readonly IShellCommand _shellCommand;

    public override ScriptEngine Engine { get; }

    public PyScriptRunner(IS7DataBlockService plcDB, IS7MBService plcMB, IS7ServerService plcServer, IShellCommand shellCommand)
    {
        this._plcDB = plcDB;
        this._plcMB = plcMB;
        this._server = plcServer;
        this._shellCommand = shellCommand;
        Engine = Python.CreateEngine();
    }

    public override ScriptScope GetScriptScope()
    {
        var scope = Engine.CreateScope();
        scope.SetVariable("s7_server_svc", this._plcDB);
        scope.SetVariable("S7", this._plcDB);

        scope.SetVariable("Server", this._server);
        scope.SetVariable("DB", this._plcDB);
        scope.SetVariable("MB", this._plcMB);
        scope.SetVariable("shell", _shellCommand);

        scope.SetVariable("__PY_ENGINE__", this.Engine);

        return scope;
    }


}
