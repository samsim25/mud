using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Avalon.Common.Scripting
{
    /// <summary>
    /// A no-op scripting engine implementation used when MoonSharp is not available.
    /// This allows the rest of the codebase to call into an IScriptEngine without
    /// requiring a compile-time dependency on the MoonSharp runtime.
    /// </summary>
    public class NoopMoonSharpEngine : IScriptEngine
    {
        public Dictionary<string, object> SharedObjects { get; set; } = new();

        public ScriptHost ScriptHost { get; set; }

        public Action<ScriptExceptionData> ExceptionHandler { get; set; }

        public NoopMoonSharpEngine(ScriptHost host)
        {
            this.ScriptHost = host;
        }

        public void RegisterObject<T>(Type t, object item, string prefix)
        {
            // no-op: store as-is so later code that inspects SharedObjects can still find it
            if (item != null && !this.SharedObjects.ContainsKey(prefix))
            {
                this.SharedObjects[prefix] = item;
            }
        }

        public T Execute<T>(string code)
        {
            return default!;
        }

        public Task<T> ExecuteAsync<T>(string code)
        {
            return Task.FromResult(default(T)!);
        }

        public Task<T> ExecuteFunctionAsync<T>(string functionName, params string[] args)
        {
            return Task.FromResult(default(T)!);
        }

        public T ExecuteFunction<T>(string functionName, params string[] args)
        {
            return default!;
        }

        public T ExecuteStatic<T>(string code)
        {
            return default!;
        }

        public Task<T> ExecuteStaticAsync<T>(string code)
        {
            return Task.FromResult(default(T)!);
        }

        public ValidationResult Validate(string code)
        {
            return new ValidationResult { Success = true };
        }

        public Task<ValidationResult> ValidateAsync(string code)
        {
            return Task.FromResult(new ValidationResult { Success = true });
        }

        public void GarbageCollect()
        {
            // no-op
        }

        public void Reset()
        {
            this.SharedObjects.Clear();
        }
    }
}
