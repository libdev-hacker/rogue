
namespace Rogue
{
    class Program
    {
        public static void Main(string[] args)
        {
            using (var window = new Window(600, 800))
            {
                window.Run();
            }
        }
    }
}