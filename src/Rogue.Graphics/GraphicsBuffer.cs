
using Veldrid;

namespace Rogue.Graphics
{
    public static class GraphicsBuffer
    {
        public static readonly GraphicsBuffer<uint> Indices = new (
            [0, 1, 3, 1, 2, 3], // From LearnOpenGL
            BufferUsage.IndexBuffer
        );
    }

    public struct GraphicsBuffer<T> (
        T[] data,
        BufferUsage type
    ) where T: unmanaged
    {
        public T[] BufferData = data;

        public BufferUsage Type = type;

        private unsafe uint _size = Convert.ToUInt32(sizeof(T) * data.Length);

        public BufferDescription Describe() => new (
            _size,
            this.Type
        );
    }
}