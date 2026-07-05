using Avalonia;
using Avalonia.Controls;

// using Rogue.Manager;

using WindowControl = Avalonia.Controls.Window;

namespace Rogue
{
    internal class Window
    {
        private struct WindowDimensions
        {
            public uint Width;
            public uint Height;
        }

        private string _title = "New Window - Rogue";

        private WindowDimensions _dimensions;

        // private TabManager _tabs; TODO: uncomment when new graphics backend is done

        public Window(uint width, uint height, string url = "")
        {
            _dimensions = new ()
            {
                Width = width,
                Height = height
            };
        }

        public void Init(Application app, string[] args)
        {
            WindowControl layout = new ()
            {
                Title = _title,
                Width = _dimensions.Width,
                Height = _dimensions.Height,
                Position = PixelPoint.Origin,
                Content = new TextBlock ()
                {
                    Text = "Hello There!",
                    FontSize = 12
                }
            };

            layout.Show();
            app.Run(layout);
        }
    }
}