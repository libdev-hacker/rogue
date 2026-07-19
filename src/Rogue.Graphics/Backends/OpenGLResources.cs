
using Veldrid;

namespace Rogue.Graphics.Backends
{
    public static class OpenGLResources
    {
        public static GraphicsDevice? Device { get; set; }

        internal static GraphicsPipelineDescription CreatePipeline() => new ()
        {
            BlendState = BlendStateDescription.SingleOverrideBlend,
            DepthStencilState = new(
                true,
                true,
                ComparisonKind.Greater // CSS z-index has greatest infront
            ),
            RasterizerState = new (
                FaceCullMode.Back,
                PolygonFillMode.Solid,
                FrontFace.CounterClockwise, // OpenGL uses Counter-Clockwise by default
                false,
                false
            )
        };
    }
}