
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Rogue.Utils.Maths
{
    public record struct BoxContainer(
        float MinimumX,
        float MinimumY,
        float MaximumX,
        float MaximumY
    )
    {
        public readonly Vector2 Size => new (this.MaximumX - this.MinimumX, this.MaximumY - this.MinimumY);

        public readonly Vector2 Centre => new (this.MinimumX + (this.Size.X / 2), this.MinimumY + (this.Size.Y / 2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPointWithin(Vector2 point) => (point.X <= this.MaximumX || point.X >= this.MinimumX) && (point.Y <= this.MaximumY || point.Y >= this.MinimumY);

        public readonly float[] GetCoords(float depth, bool hasTexture = false)
        {
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

                coords[components*i + (counter++ % components)] = this.Centre.X + (xModifier * this.Size.X / 2); // X-coordinate
                coords[components*i + (counter++ % components)] = this.Centre.Y + (yModifier * this.Size.Y / 2); // Y-coordinate
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