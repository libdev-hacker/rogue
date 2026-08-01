
using SixLabors.Fonts;
using SixLabors.Fonts.Rendering;

using Msdfgen;

using FontFillRule = SixLabors.Fonts.Rendering.FillRule;
using Vec2 = System.Numerics.Vector2;

namespace Rogue.Graphics.Text
{
    internal partial class TextRenderer: IGlyphRenderer
    {
        public Shape GlyphShape { get; private set; } = new ();

        public static readonly Msdfgen.Range Range = new (1 / 4d);

        private FontRectangle _bounds;

        private List<Contour> _contours = [];

        private Contour? _currentContour;

        private Vector2 _currentPoint;

        public void BeginText(in FontRectangle textBounds) => _bounds = textBounds;

        public void EndText()
        {
            this.GlyphShape.OrientContours();
            this.GlyphShape.Normalize();

            EdgeColoring.EdgeColoringSimple(this.GlyphShape, 3.0f); // 3 is default angle threshold from msdfgen-sharp cli library
        }

        public bool BeginGlyph(in FontRectangle glyphBounds, in GlyphRendererParameters opts) => _bounds == glyphBounds; // Purpose is to only render individual glyphs

        public void EndGlyph() => this.EndText();

        public void BeginLayer(Paint? brush, FontFillRule filler, ClipQuad? quad) {}

        public void EndLayer() {}

        public void BeginFigure()
        {
            Contour newContour = new ();
            
            _contours.Add(newContour);
            _currentContour ??= newContour;
        }

        public void MoveTo(Vec2 point) => _currentPoint = point.ToMsdf();

        public void LineTo(Vec2 point)
        {
            Vector2 newPoint = point.ToMsdf();

            LinearSegment edge = new (_currentPoint, newPoint);
            _currentContour?.AddEdge(edge);

            _currentPoint = newPoint;
        }

        public void QuadraticBezierTo(Vec2 control, Vec2 point)
        {
            Vector2 newControl, newPoint;

            (newControl, newPoint) = (control.ToMsdf(), point.ToMsdf());

            QuadraticSegment edge = new (_currentPoint, newControl, newPoint);
            _currentContour?.AddEdge(edge);

            _currentPoint = newPoint;
        }

        public void CubicBezierTo(Vec2 firstControl, Vec2 secondControl, Vec2 point)
        {
            Vector2 newFirst, newSecond, newPoint;

            (newFirst, newSecond, newPoint) = (firstControl.ToMsdf(), secondControl.ToMsdf(), point.ToMsdf());

            CubicSegment edge = new (_currentPoint, newFirst, newSecond, newPoint);
            _currentContour?.AddEdge(edge);

            _currentPoint = newPoint;
        }

        public void ArcTo(float width, float height, float rotation, bool large, bool sweep, Vec2 point)
        {
            // Construct out of bezier curves (when i found out how)
        }

        public void EndFigure()
        {
            if (_currentContour is not null) this.GlyphShape.AddContour(_currentContour);
            _currentContour = null;
        }

        public TextDecorations EnabledDecorations() => TextDecorations.None;

        public void SetDecoration(TextDecorations decorations, Vec2 start, Vec2 end, float thickness) {}

    }

    file static class VectorInteropExtensions
    {
        extension (Vec2 vector)
        {
            public Vector2 ToMsdf() => new (vector.X, vector.Y);
        }
    }
}