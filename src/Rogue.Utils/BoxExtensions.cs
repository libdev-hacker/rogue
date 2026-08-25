using OpenTK.Mathematics;

namespace Rogue.Utils
{
    public static class BoxExtensions
    {

        extension (Box2i box)
        {
            public float[] GetCoords(float depth, bool hasTexture = false)
            {
                Vector2i size = box.Size;
                Vector2 midPoint = box.Center;
                
                int components = hasTexture ? 5 : 3; // XYZ or XYZ + UV
                const int vertices = 4; // # of vertices
                Span<float> coords = stackalloc float[components * vertices];

                for (ushort i = 0; i < vertices; i++)
                {
                    float xModifier = i < 2 ? 1.0f : -1.0f;
                    float yModifier = i == 0 || i == 3 ? 1.0f : -1.0f;

                    float u = i < 2 ? 1.0f : 0.0f;
                    float v = i == 0 || i == 3 ? 1.0f : 0.0f;

                    int counter = 0;

                    coords[components*i + (counter++ % components)] = midPoint.X + (xModifier * size.X / 2); // X-coordinate
                    coords[components*i + (counter++ % components)] = midPoint.Y + (yModifier * size.Y / 2); // Y-coordinate
                    coords[components*i + (counter++ % components)] = depth; // Z-coordinate

                    if (hasTexture)
                    {
                        coords[components*i + (counter++ % components)] = u; // U component
                        coords[components*i + (counter++ % components)] = v; // V component
                    }
                }

                return coords.ToArray();
            }
            
        }
    }
}