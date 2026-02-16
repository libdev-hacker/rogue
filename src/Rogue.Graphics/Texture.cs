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
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, 0);

            // Loading texture into GPU
            image.ProcessPixelRows(image =>
            {
                List<byte> pixels = [];
                for (int y = 0; y < image.Height; y++)
                {
                    Span<Rgba32> row = image.GetRowSpan(y);
                    for (int i = 0; i < image.Width; i++)
                    {
                        pixels.Add(row[i].R);
                        pixels.Add(row[i].G);
                        pixels.Add(row[i].B);
                        pixels.Add(row[i].A);
                    }
                    GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, image.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());
                    pixels.Clear();
                }
            });

            SetParameters();

            // Adding texture coords
            var existingCoords = new List<KeyValuePair<ArraySegment<float>, float[]>>();
            const int offset = 3;
            for (int i = 0; i < coords.Length; i += offset)
            {
                float xCoord = i == 0 | i == 3 ? 1.0f : 0.0f;
                float yCoord = i == 0 | i == 9 ? 1.0f : 0.0f;

                var coord = new ArraySegment<float>(coords, i, offset);
                float[] textCoord = [xCoord, yCoord];
                existingCoords.Add(new (coord, textCoord));
            }

            List<float> newCoords = [];
            foreach (var coord in existingCoords)
            {
                foreach (float position in coord.Key)
                {
                    newCoords.Add(position);
                }
                newCoords.AddRange(coord.Value);
            }

            coords = newCoords.ToArray();
            GL.BindTexture(TextureTarget.Texture2D, 0);

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