using Microsoft.Scripting.Hosting;
using System;
using System.Threading;

namespace S7Sim.Services.Scripts
{
    public interface IScriptRunner
    {
        ScriptEngine Engine { get; }
        void RunFile(string filename, bool newScope = false, CancellationToken token = default);
        ScriptScope GetScriptScope();
    }
}
