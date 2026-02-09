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
        private TabManager _tabs;
        
        public Window(int width, int height, string url): base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize=Window.FixDimensions(width, height), Title = "Rogue", Vsync = VSyncMode.On })
        {
            _tabs = new ();
            _tabs.CreateTab(url);
            Vector2i dimensions = Window.FixDimensions(width, height);
            Shader.Orthogonal = Matrix4.CreateOrthographic(dimensions.X, dimensions.Y, -1.0f, 1.0f);
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

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            Vector2i dimensions = Window.FixDimensions(e.Width, e.Height);

            GL.Viewport(0, 0, dimensions.X, dimensions.Y); // Consider removing to keep elements static
            Shader.Orthogonal = Matrix4.CreateOrthographic(dimensions.X, dimensions.Y, -1.0f, 1.0f);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            WebPage currentPage = _tabs.Current.Value;
            currentPage.CleanUp();
        }

        private static Vector2i FixDimensions(int width, int height)
        {
            int newWidth = width % 2 == 0 ? width : width - 1;
            int newHeight = height % 2 == 0 ? height : height - 1;

            return new (newWidth, newHeight);
        }
    }
}