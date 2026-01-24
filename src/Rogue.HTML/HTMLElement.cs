using OpenTK.Mathematics;

namespace Rogue.HTML
{
    public class HTMLElement
    {
        public Vector2i Dimensions { get; set; }

        public Vector2i Location { get; set; }

        public string TagName { get; set; } = "";

        public Dictionary<string, string> Attributes { get; } = [];

        public bool IsRoot { get => this.Parent == null; }

        public HTMLElement? Parent { get; set; }

        public List<HTMLElement> Children { get; } = [];

        protected Box2i Container { get => new (this.Location.X, this.Location.Y-this.Dimensions.Y, this.Location.X+this.Dimensions.X, this.Dimensions.Y); }

        public HTMLElement()
        {
            this.Dimensions = Vector2i.Zero;
            this.Location = Vector2i.Zero;
        }

        public HTMLElement(int width, int height, int x, int y)
        {
            this.Dimensions = new (width, height);
            this.Location = new (x, y);
        }

        public virtual void Draw() { }

        public virtual void AddText(string text) { }

        public void AddChild(HTMLElement childNode)
        {
            this.Children.Add(childNode);
        }

        public void AddAttribute(string name, string value)
        {
            this.Attributes.Add(name, value);
        }
    }
}