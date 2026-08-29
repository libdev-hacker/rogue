
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using Rogue.Graphics;
using Rogue.JS;
using Rogue.Utils.Maths;

namespace Rogue.HTML
{
    public class HTMLElement
    {
        public Vector2 Dimensions { get; set; }

        public Vector2 Location { get
            {
                if (this.Parent is not null)
                {
                    return new (this.Parent.Location.X, this.Parent.Location.Y + this.Parent.Dimensions.Y);
                }
                return Vector2.Zero;
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

        protected BoxContainer Container => new (this.Location.X, this.Location.Y+this.Dimensions.Y, this.Location.X+this.Dimensions.X, this.Location.Y);

        protected DrawingContext Renderer = new ();

        public virtual void Draw()
        {
        }

        public virtual void AddText(string text) { }

        public virtual void Click(JsEngine? engine = null) {  }

        public bool IsPointWithin(Vector2 point) => this.Container.IsPointWithin(point);

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