using Rogue.HTML;

using SixLabors.ImageSharp;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;

using OpenTK.Mathematics;

namespace Rogue.Graphics
{
    public static class TextRenderer
    {
        public static int CreateText(HTMLTextElement element, ref float[] coords)
        {
            int handle;
            float[] newCoords = coords;

            FontRectangle textDimensions = TextRenderer.MeasureText(element.Text, out RichTextOptions opts);

            using (Image<Rgba32> image = new (Convert.ToInt32(textDimensions.Width), Convert.ToInt32(textDimensions.Height)))
            {
                image.Mutate(i => i.DrawText(opts, element.Text, new SolidBrush(Color.Black)).BackgroundColor(Color.White));
                handle = Texture.CreateTexture(image, ref newCoords);
            }

            coords = newCoords;
            
            return handle;
        }

        public static Vector2i MeasureText(string text)
        {
            FontRectangle dimensions = TextRenderer.MeasureText(text, out _);
            return new (Convert.ToInt32(dimensions.Width), Convert.ToInt32(dimensions.Height));
        }

        private static FontRectangle MeasureText(string text, out RichTextOptions opts)
        {

            bool hasFont = SystemFonts.TryGet("Segoe UI", out _); // Temporary default font
            if (!hasFont) Console.WriteLine("Font not found!"); // Temporary error handling / test

            const float defaultSize = 12.0f;
            Font font = SystemFonts.CreateFont("Segoe UI", defaultSize);

            opts = new (font)
            {
                Dpi = Window.HorizontalDpi
            };

            const int padding = 20;
            FontRectangle textSize = TextMeasurer.MeasureSize(text, opts);
            FontRectangle paddedSize = new (textSize.X, textSize.Y, textSize.Width+padding, textSize.Height+padding);

            return paddedSize;
        }
    }
}