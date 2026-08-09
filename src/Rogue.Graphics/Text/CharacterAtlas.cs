
using Veldrid;

using Rogue.Graphics.Backends;

using System.Text;

using SixLabors.Fonts;

namespace Rogue.Graphics.Text
{
    public class CharacterAtlas
    {
        public TextureView Texture { get; internal set; }

        public Font AtlasFont { get; }

        private List<Rune> _runes = [];

        private GraphicsDevice _device = OpenGLResources.Device ?? throw new Exception($"No {nameof(OpenGLResources.Device)} found");

        internal CharacterAtlas(Texture atlas, Font font)
        {
            this.Texture = _device.ResourceFactory.CreateTextureView(atlas);
            this.AtlasFont = font;
        }

        public int LookupRune(Rune rune) => _runes.BinarySearch(rune);

        public float CalculateOffset(Rune rune)
        {
            float index = this.LookupRune(rune);
            if (index < 0) throw new ArgumentOutOfRangeException($"Rune {rune} does not exist in atlas");

            return TextRenderer.Dimension * index / this.Texture.Target.Width;
        }

        internal void AddExistingRunes(params Rune[] runes) => _runes.AddRange(runes);
    }
}