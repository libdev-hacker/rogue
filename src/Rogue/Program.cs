
namespace Rogue
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: rogue <url>");
            } else
            {
                using (var window = new Window(600, 800, args[0]))
                {
                    window.Run();
                }
            }
        }
    }
}