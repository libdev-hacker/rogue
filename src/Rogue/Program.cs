using Avalonia;

namespace Rogue
{
    class Program
    {
        public static void Main(string[] args)
        {
            Window window = new (600, 800);

            AppBuilder.Configure<Application>().UsePlatformDetect().Start(window.Init, args);
        }
    }
}