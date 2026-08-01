
using Msdfgen;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Rogue.Graphics.Text
{
    internal static class BitmapImage
    {

        // Inspired by the Save method of the ImageSaver class in Msdfgen.Extensions package (https://github.com/ExtraBinoss/MSDFGen-Sharp/blob/main/Msdfgen.Extensions/ImageSaver.cs)
        // Originally licensed under the MIT License
        public static Image<Rgba32> GenerateImage(Bitmap<float> bitMap)
        {
            Image<Rgba32> image = new (bitMap.Width, bitMap.Height);

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);

                    for (int x = 0; x < accessor.Width; x++)
                    {
                        byte red = Convert.ToByte(BitmapImage.Clamp(bitMap[x, y, 0]));
                        byte green = Convert.ToByte(BitmapImage.Clamp(bitMap[x, y, 1]));
                        byte blue = Convert.ToByte(BitmapImage.Clamp(bitMap[x, y, 2]));

                        pixelRow[x] = new Rgba32(red, green, blue, byte.MaxValue);
                    }
                }
            });

            return image;
        }

        private static float Clamp(float value) => Math.Clamp(value * 255f, 0, 255f);
    }
}