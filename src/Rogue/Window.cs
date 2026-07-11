using Avalonia;
using Avalonia.Controls;

using Rogue.Controls;
using Rogue.Graphics;
using Rogue.Graphics.Backends;
using Rogue.Manager;

using Veldrid;

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

            GraphicsDeviceOptions opts = new ()
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true,
                Debug = true
            }; // Defaults taken from veldrid.dev tutorial

            OpenGLResources.Device ??= GraphicsDevice.CreateOpenGL(opts, _egl.GetOpenGLInfo(), width, height);

            Window.SetupDevice(OpenGLResources.Device);

            _tabs = new ();
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
                    PlatformInfo = _egl.GetOpenGLInfo()
                }
            };

            layout.Show();
            app.Run(layout);
        }

        public static void SetupDevice(GraphicsDevice device)
        {
            DeviceBuffer indexBuffer = device.ResourceFactory.CreateBuffer(GraphicsBuffer.Indices.Describe());
            device.UpdateBuffer(indexBuffer, 0, GraphicsBuffer.Indices.BufferData);

            unsafe
            {
                device.GetOpenGLInfo().DebugProc += OpenGlDebug.DebugCallback;
            }
        }
    }
}