using Rogue.HTML;

using SixLabors.ImageSharp;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace Rogue.Graphics
{
    public static class TextRenderer
    {
        public static int CreateText(HTMLTextElement element, ref float[] coords)
        {
            int handle;
            float[] newCoords = coords;

            using (Image<Rgba32> image = new (element.Dimensions.X, element.Dimensions.Y))
            {
                bool hasFont = SystemFonts.TryGet("Segoe UI", out FontFamily fontFamily); // Temporary default font
                if (!hasFont) Console.WriteLine("Font not found!"); // Temporary error handling / test

                const float defaultSize = 12.0f;
                Font font = SystemFonts.CreateFont("Segoe UI", defaultSize);

                RichTextOptions opts = new (font)
                {
                    Dpi = 72,
                    WrappingLength = element.Dimensions.X
                };

                FontRectangle dimensions = TextMeasurer.MeasureSize(element.Text, opts);

                image.Mutate(i => i.DrawText(opts, element.Text, new SolidBrush(Color.Black)).BackgroundColor(Color.White));

                handle = Texture.CreateTexture(image, ref newCoords);
            }

            coords = newCoords;
            
            return handle;
        }
    }
}