using OpenTK.Graphics.OpenGL4;

namespace Rogue.Graphics
{
    public struct Buffer
    {
        public int Handle { get; }

        public BufferTarget BufferType { get; }

        public static uint[] Indices = [
            0, 1, 2,
            1, 2, 3
        ]; // From LearnOpenGL

        public Buffer(float[] data, BufferTarget kind, BufferUsageHint hint)
        {
            this.Handle = GL.GenBuffer();
            this.BufferType = kind;

            GL.BufferData(kind, data.Length * sizeof(float), data, hint);
        }

        public Buffer(BufferUsageHint hint, BufferTarget kind = BufferTarget.ElementArrayBuffer)
        {
            this.Handle = GL.GenBuffer();
            this.BufferType = kind;

            GL.BufferData(kind, Buffer.Indices.Length * sizeof(uint), Buffer.Indices, hint);
        }
    }
}