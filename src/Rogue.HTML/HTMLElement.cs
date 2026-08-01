using System.Diagnostics.CodeAnalysis;

using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using Rogue.Graphics;
using Rogue.JS;

namespace Rogue.HTML
{
    public class HTMLElement
    {
        public Vector2i Dimensions { get; set; }

        public Vector2i Location { get
            {
                if (this.Parent is not null)
                {
                    return new (this.Parent.Location.X, this.Parent.Location.Y + this.Parent.Dimensions.Y);
                }
                return Vector2i.Zero;
            }
        }

        public string TagName { get; set; } = "";

        public Dictionary<string, string> Attributes { get; } = [];

        [MemberNotNullWhen(false, nameof(Parent))]
        public bool IsRoot { get => this.Parent == null; }

        public HTMLElement? Parent { get; set; }

        public List<HTMLElement> Children { get; } = [];

        public bool HasEndTag { get; protected set; } = true;

        protected float Depth;

        protected Box2i Container { get => new (this.Location.X, this.Location.Y+this.Dimensions.Y, this.Location.X+this.Dimensions.X, this.Location.Y); }

        protected DrawingContext Renderer = new ();

        public HTMLElement()
        {
            this.Dimensions = Vector2i.Zero;
        }

        public HTMLElement(int width, int height, int x, int y)
        {
            this.Dimensions = new (width, height);
        }

        public virtual void Draw()
        {
        }

        public virtual void AddText(string text) { }

        public virtual void Click(JsEngine? engine = null) {  }

        public bool IsPointWithin(Vector2i point) => this.Container.ContainsInclusive(point);

        public void AddChild(HTMLElement childNode)
        {
            childNode.Depth = this.Depth + 0.01f;
            this.Children.Add(childNode);
        }

        public void AddAttribute(string name, string value)
        {
            this.Attributes.Add(name, value);
        }

        public void EndDraw()
        {
            this.Renderer.Dispose();
        }
    }
}