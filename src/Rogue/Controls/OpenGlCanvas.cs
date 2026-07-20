
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;

using SkiaSharp;

using Veldrid;
using Veldrid.OpenGL;
using Veldrid.OpenGLBinding;

using Rogue.Graphics.Backends;

namespace Rogue.Controls
{
    public class OpenGlCanvas: Control, ICustomDrawOperation
    {
        public required OpenGLPlatformInfo PlatformInfo;

        private Framebuffer? _fbo = OpenGLResources.Device?.SwapchainFramebuffer;

        private BackendInfoOpenGL? _backendInfo = OpenGLResources.Device?.GetOpenGLInfo();

        public override void Render(DrawingContext context) => context.Custom(this);

        public void Render(ImmediateDrawingContext context)
        {
            if (_fbo is not null && _backendInfo is not null)
            {
                var feature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
                if (feature is null) return;

                using ISkiaSharpApiLease lease = feature.Lease();
                SKCanvas canvas = lease.SkCanvas;

                using GRContext oglContext = GRContext.CreateGl(GRGlInterface.CreateOpenGl(this.PlatformInfo.GetProcAddress.Invoke));
                
                Texture fboTexture = _fbo.ColorTargets[0].Target;
                uint nativeTextureHandle = _backendInfo.GetTextureName(fboTexture);
                
                GRGlTextureInfo skiaTextureInfo = new (
                    OpenGlCanvas.ToOpenGLTarget(fboTexture.Type),
                    nativeTextureHandle
                );

                using GRBackendTexture skiaTexture = new ((int) fboTexture.Width, (int) fboTexture.Height, fboTexture.MipLevels > 0, skiaTextureInfo);

                using SKImage result = SKImage.FromTexture(oglContext, skiaTexture, SKColorType.Unknown);

                canvas.DrawImage(result, 0, 0);
            }
        }

        private static uint ToOpenGLTarget(TextureType type) => type switch
        {
            TextureType.Texture1D => (uint) TextureTarget.Texture1D,
            TextureType.Texture2D => (uint) TextureTarget.Texture2D,
            TextureType.Texture3D => (uint) TextureTarget.Texture3D,
            _ => throw new Exception("Unknown Texture Target")
        };

        public bool HitTest(Point p) => this.Bounds.Contains(p);

        public bool Equals(ICustomDrawOperation? op) => false;

        public void Dispose() { }
    }
}