using Avalonia;

namespace Rogue
{
    class Program
    {
        public static void Main(string[] args)
        {
            Window window = args.Length == 0 ? new (600, 800) : new (600, 800, args[0]);

            AppBuilder.Configure<Application>().UsePlatformDetect().Start(window.Init, args);
        }
    }
}