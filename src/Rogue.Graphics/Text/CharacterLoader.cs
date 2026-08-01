
using System.Collections.Concurrent;
using System.Text;

using Veldrid;
using Veldrid.ImageSharp;

using Rogue.Graphics.Backends;
using Rogue.Utils;

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using Msdfgen;

namespace Rogue.Graphics.Text
{
    public class CharacterLoader(
        Font font
    )
    {
        public static readonly ConcurrentDictionary<Font, Texture> CharAtlases = [];

        private SafeHashSet<Rune> _chars = [];

        private Font _font = font;

        private GraphicsDevice _device = OpenGLResources.Device ?? throw new Exception("No GraphicsDevice found!");

        public static CharacterLoader LoadDefaultFont() => CharacterLoader.LoadAsciiFromFont(TextRenderer.DefaultFontOptions.Font);

        public static CharacterLoader LoadAsciiFromFont(Font targetFont)
        {
            Rune[] asciiRange = Enumerable.Range(char.MinValue, char.MaxValue).Where(c => !char.IsControl((char)c) && char.IsAscii((char) c) && !char.IsWhiteSpace((char) c)).Select(c => new Rune(c)).ToArray();

            CharacterLoader loader = new (targetFont);

            Parallel.ForEach(asciiRange, loader.AddCharacter);

            return loader;
        }

        private TextureDescription GetDescription(TextureUsage type) => new (
            Convert.ToUInt32(TextRenderer.Dimension * _chars.Count),
            TextRenderer.Dimension,
            1,
            1,
            1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            type,
            TextureType.Texture2D
        );

        public void AddCharacter(Rune character)
        {
            var newSet = _chars.Add(character);

            if (newSet.SetEquals(_chars)) return;

            Texture currentAtlas = _device.ResourceFactory.CreateTexture(this.GetDescription(TextureUsage.Sampled));

            Bitmap<float> bitmap = TextRenderer.RenderCharacter(character, new TextOptions(_font));

            using Image<Rgba32> image = BitmapImage.GenerateImage(bitmap);

            using Texture characterTexture = new ImageSharpTexture(image, false).CreateDeviceTexture(_device, _device.ResourceFactory);

            using CommandList commands = _device.ResourceFactory.CreateCommandList(
                new CommandListDescription()
                {
                    Transient = true
                }
            );

            if (_chars.Count == 1)
            {
                commands.Begin();

                commands.CopyTexture(characterTexture, currentAtlas);

                commands.End();
                _device.SubmitCommands(commands);
                _device.WaitForIdle();

                CharacterLoader.CharAtlases[_font] = currentAtlas;

            } else
            {
                Texture newAtlas = _device.ResourceFactory.CreateTexture(this.GetDescription(TextureUsage.Sampled));
                
                commands.Begin();

                commands.CopyTexture(
                    currentAtlas,
                    0,
                    0,
                    0,
                    0,
                    0,
                    newAtlas,
                    0,
                    0,
                    0,
                    0,
                    0,
                    currentAtlas.Width,
                    currentAtlas.Height,
                    1,
                    1
                );

                commands.CopyTexture(
                    characterTexture,
                    0,
                    0,
                    0,
                    0,
                    0,
                    newAtlas,
                    (uint) _chars.Count-1,
                    0,
                    0,
                    0,
                    0,
                    (uint) bitmap.Width,
                    (uint) bitmap.Height,
                    1,
                    1
                );

                commands.End();
                _device.SubmitCommands(commands);
                _device.WaitForIdle();

                CharacterLoader.CharAtlases[_font] = newAtlas;
            }
        }

        public int LookupRune(Rune rune) => _chars.ToList().IndexOf(rune);
    }
}