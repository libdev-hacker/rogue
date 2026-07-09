using Avalonia;
using Avalonia.Controls;

using Rogue.Controls;
using Rogue.Graphics.Backends;
using Rogue.Manager;

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

        private readonly EglInfo _egl = new ();

        private readonly TabManager _tabs;

        public Window(uint width, uint height, string url = "")
        {
            _dimensions = new ()
            {
                Width = width,
                Height = height
            };

            OpenGLInfo info = new (_egl.GetOpenGLInfo(), width, height);
            _tabs = new (info);

            _tabs.CreateTab(url, true);
        }

        public void Init(Application app, string[] args)
        {
            WindowControl layout = new ()
            {
                Title = _title,
                Width = _dimensions.Width,
                Height = _dimensions.Height,
                Position = PixelPoint.Origin,
                Content = new OpenGlCanvas()
                {
                    PlatformInfo = _egl.GetOpenGLInfo(),
                    Fbo = _tabs.Graphics?.SwapchainFramebuffer ?? throw new Exception("No Swapchain found"),
                    BackendInfo = _tabs.Graphics.GetOpenGLInfo()
                }
            };

            layout.Show();
            app.Run(layout);
        }
    }
}