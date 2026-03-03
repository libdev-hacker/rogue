using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;

using Rogue.Graphics;

namespace Rogue
{
    public class Window: GameWindow
    {
        // Title is inherited from GameWindow

        public static float HorizontalDpi { get; private set; }

        private TabManager _tabs;
        
        public Window(int width, int height, string url): base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize=Window.FixDimensions(width, height), Title = "Rogue", Vsync = VSyncMode.On, WindowBorder = WindowBorder.Fixed })
        {
            _tabs = new ();
            _tabs.CreateTab(url);

            Vector2i dimensions = Window.FixDimensions(width, height);
            Shader.Orthogonal = Matrix4.CreateOrthographicOffCenter(0, dimensions.X, dimensions.Y, 0, -1, 1);

            if (this.TryGetCurrentMonitorDpi(out float dpi, out _))
            {
                Window.HorizontalDpi = dpi;
            } else
            {
                Console.WriteLine("DPI not found!");
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            WebPage currentPage = _tabs.Current.Value;
            currentPage.RenderPage();

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            WebPage currentPage = _tabs.Current.Value;
            currentPage.CleanUp();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            WebPage currentPage = _tabs.Current.Value;
            Vector2 pos = this.MousePosition;
            currentPage.RegisterClick(this.PointToClient(new (Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y))));
        }

        private static Vector2i FixDimensions(int width, int height)
        {
            int newWidth = width % 2 == 0 ? width : width - 1;
            int newHeight = height % 2 == 0 ? height : height - 1;

            return new (newWidth, newHeight);
        }
    }
}