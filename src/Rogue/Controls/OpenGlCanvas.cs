
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;

using SkiaSharp;

using Veldrid;
using Veldrid.OpenGL;

using Rogue.Graphics.Backends;
using System.Runtime.CompilerServices;

namespace Rogue.Controls
{
    public class OpenGlCanvas: Control, ICustomDrawOperation
    {
        public required OpenGLPlatformInfo PlatformInfo;

        public required Action RenderProc;

        public override void Render(DrawingContext context) => context.Custom(this);

        public void Render(ImmediateDrawingContext context)
        {
            this.PlatformInfo.MakeCurrent(this.PlatformInfo.OpenGLContextHandle);
            this.RenderProc.Invoke();

            Framebuffer fbo = OpenGLResources.MainFrameBuffer ?? throw new Exception("No Framebuffer found");
            BackendInfoOpenGL? backendInfo = OpenGLResources.Device?.GetOpenGLInfo();

            if (backendInfo is not null)
            {
                var feature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
                if (feature is null) return;

                using ISkiaSharpApiLease lease = feature.Lease();
                SKCanvas canvas = lease.SkCanvas;

                using GRContext backendContext = GRContext.CreateGl(GRGlInterface.CreateOpenGl(this.PlatformInfo.GetProcAddress.Invoke));

                Texture fboTexture = fbo.ColorTargets[0].Target;
                uint nativeTextureHandle = backendInfo.GetTextureName(fboTexture);

                using GRBackendRenderTarget skiaFboTarget = new (
                    (int) fboTexture.Width,
                    (int) fboTexture.Height,
                    OpenGlCanvas.GetSampleCount(fbo.OutputDescription.SampleCount),
                    stencilBits: 0,
                    new GRGlFramebufferInfo(
                        nativeTextureHandle,
                        SKColorType.Rgba8888.ToGlSizedFormat()
                    )
                );

                using SKSurface surface = SKSurface.Create(
                    backendContext,
                    skiaFboTarget,
                    SKColorType.Rgba8888
                );

                canvas.DrawSurface(surface, 0, 0);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetSampleCount(TextureSampleCount sampleCount) => sampleCount switch
        {
            TextureSampleCount.Count1 => 1,
            TextureSampleCount.Count2 => 2,
            TextureSampleCount.Count4 => 4,
            TextureSampleCount.Count8 => 8,
            TextureSampleCount.Count16 => 16,
            TextureSampleCount.Count32 => 32,
            TextureSampleCount.Count64 => 64,
            _ => throw new Exception("Unknown Texture Sample Count")
        };

        public bool HitTest(Point p) => this.Bounds.Contains(p);

        public bool Equals(ICustomDrawOperation? op) => false;

        public void Dispose() { }
    }
}