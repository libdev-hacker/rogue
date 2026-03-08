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

            RichTextOptions opts = TextRenderer.GenerateFont();

            using (Image<Rgba32> image = new (element.Dimensions.X, element.Dimensions.Y))
            {
                image.Mutate(i => i.DrawText(opts, element.InnerText.Text, new SolidBrush(Color.Black)).BackgroundColor(Color.White));
                handle = Texture.CreateTexture(image, ref newCoords);
            }

            coords = newCoords;
            
            return handle;
        }

        public static int CreateText(string text, ref float[] coords, Color bgColour)
        {
            int handle;
            float[] newCoords = coords;

            FontRectangle textDimensions = TextRenderer.MeasureText(text, out RichTextOptions opts);

            using (Image<Rgba32> image = new (Convert.ToInt32(textDimensions.Width), Convert.ToInt32(textDimensions.Height)))
            {
                image.Mutate(i => i.DrawText(opts, text, new SolidBrush(Color.Black)).BackgroundColor(bgColour));
                handle = Texture.CreateTexture(image, ref newCoords);
            }

            coords = newCoords;

            return handle;
        }

        public static int CreateText(string text, ref float[] coords) => TextRenderer.CreateText(text, ref coords, Color.White);

        public static Vector2i MeasureText(string text)
        {
            FontRectangle dimensions = TextRenderer.MeasureText(text, out _);
            return new (Convert.ToInt32(dimensions.Width), Convert.ToInt32(dimensions.Height));
        }

        private static FontRectangle MeasureText(string text, out RichTextOptions opts)
        {
            const int padding = 20;
            opts = TextRenderer.GenerateFont();

            FontRectangle textSize = TextMeasurer.MeasureSize(text, opts);
            FontRectangle paddedSize = new (textSize.X, textSize.Y, textSize.Width+padding, textSize.Height+padding);

            return paddedSize;
        }

        private static RichTextOptions GenerateFont()
        {
            bool hasFont = SystemFonts.TryGet("Segoe UI", out _); // Temporary default font
            if (!hasFont) Console.WriteLine("Font not found!"); // Temporary error handling / test

            const float defaultSize = 12.0f;
            Font font = SystemFonts.CreateFont("Segoe UI", defaultSize);

            return new (font)
            {
                Dpi = Window.HorizontalDpi
            };
        }
    }
}