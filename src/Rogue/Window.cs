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
        
        public Window(int width, int height): base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize=new (width, height), Title = "Rogue", Vsync = VSyncMode.On })
        {
            _tabs = new ();
            Shader.Orthogonal = Matrix4.CreateOrthographicOffCenter(0.0f, width, 0.0f, height, 0.0f, 1.0f);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            SwapBuffers();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            Shader.Orthogonal = Matrix4.CreateOrthographicOffCenter(0.0f, e.Width, 0.0f, e.Height, 0.0f, 1.0f);
        }
        
    }
}