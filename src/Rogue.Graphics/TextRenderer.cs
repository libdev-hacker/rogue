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

            using (Image<Rgba32> image = new (element.Dimensions.X, element.Dimensions.Y))
            {
                bool hasFont = SystemFonts.TryGet("Arial", out FontFamily fontFamily); // Temporary default font
                if (!hasFont) Console.WriteLine("Font not found!"); // Temporary error handling / test

                Font font = fontFamily.CreateFont(24);

                RichTextOptions opts = new (font)
                {
                    Dpi = 72,
                    WrappingLength = element.Dimensions.X
                };

                FontRectangle dimensions = TextMeasurer.MeasureSize(element.Text, opts);

                image.Mutate(i => i.DrawText(opts, element.Text, new SolidBrush(Color.Black)));

                handle = Texture.CreateTexture(image, ref coords);
            }
            
            return handle;
        }
    }
}