
using Veldrid;
using Veldrid.OpenGL;

namespace Rogue.Graphics.Backends
{
    public readonly struct OpenGLInfo(
        OpenGLPlatformInfo info,
        uint width,
        uint height
    )
    {
        public GraphicsDeviceOptions Opts { get; init; } = new ()
        {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true
        }; // Defaults taken from veldrid.dev tutorial

        public OpenGLPlatformInfo Info { get; } = info;

        public uint Width { get; } = width;

        public uint Height { get; } = height;
    }
}