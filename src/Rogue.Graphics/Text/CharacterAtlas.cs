
using Veldrid;

using System.Text;

using SixLabors.Fonts;

namespace Rogue.Graphics.Text
{
    public class CharacterAtlas
    {
        public Texture Texture { get; internal set; }

        public Font AtlasFont { get; }

        private List<Rune> _runes = [];

        internal CharacterAtlas(Texture atlas, Font font)
        {
            this.Texture = atlas;
            this.AtlasFont = font;
        }

        public int LookupRune(Rune rune) => _runes.BinarySearch(rune);

        public float CalculateOffset(Rune rune)
        {
            float index = this.LookupRune(rune);
            if (index < 0) throw new ArgumentOutOfRangeException($"Rune {rune} does not exist in atlas");

            return TextRenderer.Dimension * index / this.Texture.Width;
        }

        internal void AddExistingRunes(params Rune[] runes)
        {
            _runes.AddRange(runes);
            _runes.Sort((x, y) => x.CompareTo(y));
        }
    }
}