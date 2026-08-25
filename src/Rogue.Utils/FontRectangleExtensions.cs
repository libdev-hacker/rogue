
using OpenTK.Mathematics;

using SixLabors.Fonts;

namespace Rogue.Utils
{
    public static class FontRectangleExtensions
    {
        extension (FontRectangle fontRectangle)
        {
            public Vector2i GetDimensions() => new (Convert.ToInt32(fontRectangle.Width), Convert.ToInt32(fontRectangle.Height));
        }
    }
}