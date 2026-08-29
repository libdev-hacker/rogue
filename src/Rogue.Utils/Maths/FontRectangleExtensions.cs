
using System.Numerics;

using SixLabors.Fonts;

namespace Rogue.Utils.Maths
{
    public static class FontRectangleExtensions
    {
        extension (FontRectangle fontRectangle)
        {
            public Vector2 GetDimensions() => new (Convert.ToInt32(fontRectangle.Width), Convert.ToInt32(fontRectangle.Height));
        }
    }
}