using Avalonia;
using Avalonia.Controls;

using Rogue.Controls;
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

        private EglManager _egl = new ();

        private TabManager _tabs;

        public Window(uint width, uint height, string url = "")
        {
            _dimensions = new ()
            {
                Width = width,
                Height = height
            };

            this.InitialiseOpenGL();

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
                    PlatformInfo = _egl.GetOpenGLInfo(),
                    RenderProc = ((WebPage) _tabs.Current).RenderPage
                }
            };

            layout.Resized += this.OnResize;

            layout.Show();
            app.Run(layout);
        }

        private void InitialiseOpenGL()
        {
            uint width = _dimensions.Width;
            uint height = _dimensions.Height;

            GraphicsDeviceOptions opts = new ()
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true,
                Debug = true
            }; // Defaults taken from veldrid.dev tutorial

            OpenGLResources.Device ??= GraphicsDevice.CreateOpenGL(opts, _egl.GetOpenGLInfo(), width, height);

            Texture fboColourTarget = OpenGLResources.Device.ResourceFactory.CreateTexture(new TextureDescription(
                width,
                height,
                1,
                1,
                1,
                PixelFormat.R8_G8_B8_A8_UNorm,
                TextureUsage.RenderTarget,
                TextureType.Texture2D
            ));

            Texture fboDepthTarget = OpenGLResources.Device.ResourceFactory.CreateTexture(new TextureDescription(
                width,
                height,
                1,
                1,
                1,
                PixelFormat.D16_UNorm,
                TextureUsage.DepthStencil,
                TextureType.Texture2D
            ));

            if (OpenGLResources.MainFrameBuffer is not null && !OpenGLResources.MainFrameBuffer.IsDisposed)
            {
                OpenGLResources.MainFrameBuffer.Dispose();
            }

            OpenGLResources.MainFrameBuffer = OpenGLResources.Device.ResourceFactory.CreateFramebuffer(new FramebufferDescription(fboDepthTarget, fboColourTarget));
        }

        private void OnResize(object? sender, WindowResizedEventArgs args)
        {
            _dimensions = new ()
            {
                Width = Convert.ToUInt32(args.ClientSize.Width),
                Height = Convert.ToUInt32(args.ClientSize.Height)
            };

            OpenGLResources.Device?.ResizeMainWindow(_dimensions.Width, _dimensions.Height);

            this.InitialiseOpenGL(); // Recreates framebuffers
        }
    }
}