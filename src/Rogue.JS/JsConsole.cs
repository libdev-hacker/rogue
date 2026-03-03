

namespace Rogue.JS
{
    public class JsConsole
    {
        public void Clear() => Console.Clear();

        public void Log(string text) => Console.WriteLine(text);

        public void Warn(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}