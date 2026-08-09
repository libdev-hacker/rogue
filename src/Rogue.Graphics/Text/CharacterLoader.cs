
using System.Text;

using Veldrid;
using Veldrid.ImageSharp;

using Rogue.Graphics.Backends;

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using Msdfgen;

namespace Rogue.Graphics.Text
{
    public static class CharacterLoader
    {
        private static readonly Dictionary<Font, CharacterAtlas> s_charAtlases = [];

        private static readonly Rune[] s_asciiRange = Enumerable.Range(char.MinValue, char.MaxValue).Where(c => !char.IsControl((char)c) && char.IsAscii((char) c) && !char.IsWhiteSpace((char) c)).Select(c => new Rune(c)).ToArray();

        public static CharacterAtlas LoadDefaultFont() => CharacterLoader.LoadAsciiFromFont(TextRenderer.DefaultFontOptions.Font);

        public static CharacterAtlas LoadAsciiFromFont(Font targetFont)
        {
            if (s_charAtlases.TryGetValue(targetFont, out CharacterAtlas? texture))
            {
                return texture;
            }

            GraphicsDevice device = OpenGLResources.Device ?? throw new Exception("No GraphicsDevice found");

            List<Task> tasks = [];
            var charTextures = new Image<Rgba32>[s_asciiRange.Length];

            Image<Rgba32> atlasImage = new (TextRenderer.Dimension * s_asciiRange.Length, TextRenderer.Dimension);

            for (int i = 0; i < s_asciiRange.Length; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    Bitmap<float> bitmap = TextRenderer.RenderCharacter(s_asciiRange[i], new TextOptions(targetFont));
                    charTextures[i] = BitmapImage.GenerateImage(bitmap);
                }));
            }

            Task t = Task.WhenAll(tasks);

            try
            {
                t.Wait();
            } catch {}

            if (t.Status == TaskStatus.RanToCompletion)
            {
                for (int i = 0; i < s_asciiRange.Length; i++)
                {
                    atlasImage.Mutate(atlas => atlas.DrawImage(charTextures[i], new Point(TextRenderer.Dimension * i, 0), 0));
                }
            }

            Texture atlas = new ImageSharpTexture(atlasImage, false).CreateDeviceTexture(device, device.ResourceFactory);
            atlas.Name = targetFont.Name;

            CharacterAtlas characterAtlasInstance = new (atlas, targetFont);
            characterAtlasInstance.AddExistingRunes(s_asciiRange);
            
            s_charAtlases[targetFont] = characterAtlasInstance;

            return characterAtlasInstance;
        }

        public static void AddCharacter(CharacterAtlas atlas, Rune rune)
        {
            GraphicsDevice device = OpenGLResources.Device ?? throw new Exception("No GraphicsDevice found");
            Texture target = atlas.Texture.Target;

            atlas.AddExistingRunes(rune);

            int newX = Convert.ToInt32(TextRenderer.Dimension + target.Width);

            MappedResource mappedImage = device.Map(target, MapMode.Read);

            Image<Rgba32> atlasImage = Image.LoadPixelData<Rgba32>(mappedImage.AsBytes(), (int) target.Width, (int) target.Height);

            Bitmap<float> charBitMap = TextRenderer.RenderCharacter(rune, new TextOptions(atlas.AtlasFont));
            Image<Rgba32> charImage = BitmapImage.GenerateImage(charBitMap);

            atlasImage.Mutate(atlas =>
            {
                atlas.Resize(newX, TextRenderer.Dimension);
                atlas.DrawImage(charImage, new Point(charImage.Width, 0), 0);
            });

            Texture newAtlasTexture = new ImageSharpTexture(atlasImage, false).CreateDeviceTexture(device, device.ResourceFactory);

            atlas.Texture = device.ResourceFactory.CreateTextureView(newAtlasTexture);
        }
    }
}