using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;

namespace Rogue
{
    public class Window: GameWindow
    {
        // Title is inherited from GameWindow
        private TabManager _tabs;
        
        public Window(int width, int height): base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize=new (width, height), Title = "Rogue" })
        {
            _tabs = new ();
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
        
    }
}