using Microsoft.Scripting.Hosting;
using System.Threading;

namespace S7Sim.Services.Scripts
{
    /// <summary>
    /// <see cref="IScriptRunner"/> Base <see langword="class"/>
    /// </summary>
    public abstract class ScriptRunner : IScriptRunner
    {
        public abstract ScriptEngine Engine { get; }
        protected ScriptScope? scope = null;

        /// <summary>
        /// <para>Run by target file.</para>
        /// <para>If set <paramref name="filePath"/> to <seealso langword="true"/>, it will always create new scope when run script.</para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="newScope"></param>
        public void RunFile(string filePath, bool newScope = false, CancellationToken token = default)
        {
            if (scope is null || newScope)
            {
                scope = GetScriptScope();
            }

            var source = Engine.CreateScriptSourceFromFile(filePath);
            var code = source.Compile();

            code.Execute(scope);
        }

        public abstract ScriptScope GetScriptScope();
    }
}
