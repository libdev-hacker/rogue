using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Rogue.Graphics
{
    public static class Texture
    {
        public static int CreateTexture(Image<Rgba32> image, ref float[] coords)
        {
            int handle = GL.GenTexture();

            // Loading texture into GPU
            image.ProcessPixelRows(image =>
            {
                int width = image.Width * 4;
                byte[] pixels = new byte[width];
                for (int y = 0; y < image.Height; y++)
                {
                    Span<Rgba32> row = image.GetRowSpan(y);
                    int counter = 0;
                    for (int i = 0; i < width; i++)
                    {
                        pixels[counter++] = row[i].R;
                        pixels[counter++] = row[i].G;
                        pixels[counter++] = row[i].B;
                        pixels[counter++] = row[i].A;
                    }
                    GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, image.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
                }
            });

            SetParameters();

            // Adding texture coords
            int length = coords.Length + (2 * 4); // length of coords + (2 new coords * 4 points)
            int coordLength = 3;
            float[] newCoords = new float[length];
            for (int i = 0; i < length / 5; i += 5)
            {
                // Variable initialisation
                int xCoord = i < length / 2 ? 1 : 0;
                int yCoord = i == 0 || i == length * 2 / 3 ? 1 : 0;
                int textCoordOffset = i+coordLength;

                // Copying element coordinates over to new array
                var elementCoords = new ArraySegment<float>(coords, i, coordLength);
                newCoords = newCoords.Concat(elementCoords).ToArray();

                // Appending texture coordinates
                newCoords[textCoordOffset] = xCoord;
                newCoords[textCoordOffset+1] = yCoord;
            }

            coords = newCoords;

            return handle;
        }

        private static void SetParameters()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, Convert.ToInt32(TextureWrapMode.ClampToBorder));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, Convert.ToInt32(TextureWrapMode.ClampToBorder));

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, Convert.ToInt32(TextureMinFilter.LinearMipmapLinear));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, Convert.ToInt32(TextureMinFilter.Linear));

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
    }
}