
using System.Text;

using Msdfgen;

using SixLabors.Fonts;

using FontRenderer = SixLabors.Fonts.Rendering.TextRenderer;

namespace Rogue.Graphics.Text
{
    internal partial class TextRenderer
    {
        public const int Dimension = 32;

        public static Bitmap<float> RenderCharacter(Rune symbol, TextOptions opts)
        {
            TextRenderer renderer = new ();
            FontRenderer.RenderTextTo(renderer, symbol.ToString(), opts);

            Bitmap<float> output = new (TextRenderer.Dimension, TextRenderer.Dimension, channels: 3);

            MsdfGenerator.GenerateMSDF(
                output,
                renderer.GlyphShape,
                TextRenderer.GetProjection(renderer.GlyphShape),
                TextRenderer.Range,
                new MSDFGeneratorConfig()
            );

            return output;
        }

        private static Projection GetProjection(Shape glyph)
        {
            // Following code was taken & adapted from ConfigureGeometry method in CliProcessor class of Msdfgen.Cli package (https://github.com/ExtraBinoss/MSDFGen-Sharp/blob/main/Msdfgen.Cli/CliProcessor.cs)
            // Originally licensed under the MIT license

            double l = 1e240, b = 1e240, r = -1e240, t = -1e240;
            glyph.Bound(ref l, ref b, ref r, ref t);
            
            if (l >= r || b >= t) { l = 0; b = 0; r = 1; t = 1; }

            double pxRange = TextRenderer.Range.Upper - TextRenderer.Range.Lower;
            Vector2 frame = new (TextRenderer.Dimension, TextRenderer.Dimension);
            frame = new Vector2(frame.X - pxRange, frame.Y - pxRange);

            if (frame.X <= 0 || frame.Y <= 0) throw new Exception("Cannot fit the specified pixel range.");

            Vector2 dims = new (r - l, t - b);

            Vector2 scale, translation;

            if (dims.X * frame.Y < dims.Y * frame.X)
            {
                double fitScale = frame.Y / dims.Y;
                translation = new (0.5 * (frame.X / frame.Y * dims.Y - dims.X) - l, -b);
                scale = new (fitScale, fitScale);
            }
            else
            {
                double fitScale = frame.X / dims.X;
                translation = new Vector2(-l, 0.5 * (frame.Y / frame.X * dims.X - dims.Y) - b);
                scale = new Vector2(fitScale, fitScale);
            }

            // Adjust for pxRange centering
            translation += new Vector2((pxRange/2)/scale.X, (pxRange/2)/scale.Y);

            return new Projection(scale, translation);
        }
    }
}