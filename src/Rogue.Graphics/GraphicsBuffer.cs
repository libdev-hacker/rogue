
using System.Runtime.InteropServices;

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

    public readonly struct GraphicsBuffer<T> (
        T[] data,
        BufferUsage type
    ) where T: unmanaged
    {
        public readonly T[] BufferData { get; } = data;

        public readonly BufferUsage Type { get; } = type;

        private readonly uint _size = Convert.ToUInt32(Marshal.SizeOf<T>() * data.Length);

        public BufferDescription Describe() => new (
            _size,
            this.Type
        );

        public uint GetByteOffset(int index) => Convert.ToUInt32(_size * (Convert.ToSingle(index) / this.BufferData.Length));
    }
}