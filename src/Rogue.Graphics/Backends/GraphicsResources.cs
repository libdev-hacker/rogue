
using Veldrid;

namespace Rogue.Graphics.Backends
{
    public static class GraphicsResources
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
                FrontFace.CounterClockwise,
                false,
                false
            ),
            PrimitiveTopology = PrimitiveTopology.TriangleList
        };
    }
}