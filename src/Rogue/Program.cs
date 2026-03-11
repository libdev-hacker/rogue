using Rogue.Utils;

namespace Rogue
{
    class Program
    {
        public static void Main(string[] args)
        {
            string url = args.Length == 0 ? WebClient.BlankPage : args[0];
            using (var window = new Window(600, 800, url))
            {
                window.Run();
            }
        }
    }
}