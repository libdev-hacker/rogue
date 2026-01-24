using OpenTK.Mathematics;

namespace Rogue.Utils
{
    public static class BoxExtensions
    {
        public static float[] GetCoords(this Box2i box, float depth)
        {
            Vector2i size = box.Size;
            Vector2 midPoint = box.Center;
            
            const ushort components = 3; // XYZ
            const ushort vertices = 4; // # of vertices
            Span<float> coords = stackalloc float[components * vertices];

            for (ushort i = 0; i < vertices; i++)
            {
                int xModifier = i < 2 ? 1 : -1;
                int yModifier = i == 0 || i == 3 ? 1 : -1;

                int counter = 0;

                coords[3*i + (counter++ % components)] = midPoint.X + (xModifier * size.X / 2); // X-coordinate
                coords[3*i + (counter++ % components)] = midPoint.Y + (yModifier * size.Y / 2); // Y-coordinate
                coords[3*i + (counter++ % components)] = depth; // Z-coordinate
            }

            return coords.ToArray();
        }
    }
}