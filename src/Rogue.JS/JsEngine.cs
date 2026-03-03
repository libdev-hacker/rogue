using Jint;
using Jint.Runtime.Interop;

namespace Rogue.JS
{
    public class JsEngine
    {
        public Engine Engine { get => _engine; }

        private readonly Engine _engine = new ();

        public void RunScript(string script) => _engine.Execute(script);

        public void InvokeMethod(string methodName) => _engine.Invoke(methodName);

        public void AddNativeClass<T>(string name) => _engine.SetValue(name, TypeReference.CreateTypeReference<T>(_engine));

        public void AddNativeObject<T>(T obj, string objectName) => _engine.SetValue<T>(objectName, obj);
    }
}