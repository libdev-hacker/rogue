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
            // Creating the default shader
            if (this.Renderer.Shader == -1)
            {
                const string vertexShader = @"
                    #version 420 core

                    layout(location = 0) in vec3 position;
                    layout(location = 1) in vec2 texCoords;

                    out vec2 texPoints;

                    uniform mat4 transform;

                    void main() {
                        gl_Position = transform * vec4(position, 1.0);
                        texPoints = texCoords;
                    }
                ";

                const string fragShader = @"
                    #version 420 core

                    out vec4 colour;

                    in vec2 texPoints;

                    uniform sampler2D texSampler;

                    void main() {
                        colour = texture(texSampler, texPoints);
                    }
                ";

                int shader = Shader.CreateShader(vertexShader, fragShader);
                if (shader == -1) return;
                this.Renderer.AddShader(shader);
            }

            // Creating OpenGL buffers
            if (!this.Renderer.IsBuffersSet)
            {
                GraphicsBuffer vbo = new (BufferTarget.ArrayBuffer);
                GraphicsBuffer ebo = new (BufferTarget.ElementArrayBuffer);
                
                this.Renderer.AddVertexBufferObject(vbo);
                this.Renderer.AddElementBufferObject(ebo);
            }

            this.Renderer.UseShader();
            Shader.SetOrthogonalMatrix(this.Renderer.Shader);

            // Setting up element so OpenGL can draw it
            this.Renderer.BindVao();
            this.Renderer.BindBuffer(BufferTarget.ArrayBuffer);
            this.Renderer.BindBuffer(BufferTarget.ElementArrayBuffer);

            this.Renderer.AddAttributePointer(0, 3, VertexAttribPointerType.Float, 5 * sizeof(float));
            this.Renderer.AddAttributePointer(1, 2, VertexAttribPointerType.Float, 5 * sizeof(float), 3 * sizeof(float));

            this.Renderer.DrawElement();

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