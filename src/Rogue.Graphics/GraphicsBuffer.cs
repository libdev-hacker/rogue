using OpenTK.Graphics.OpenGL4;

namespace Rogue.Graphics
{
    public struct GraphicsBuffer
    {
        public int Handle { get; }

        public BufferTarget BufferType { get; }

        public static uint[] Indices = [
            0, 1, 3,
            1, 2, 3
        ]; // From LearnOpenGL

        public GraphicsBuffer(BufferTarget kind)
        {
            this.Handle = GL.GenBuffer();
            this.BufferType = kind;
        }
    }
}