using System.Text;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using Rogue.Graphics;
using Rogue.Utils;

namespace Rogue.HTML
{
    public class HTMLTextElement: HTMLElement
    {
        private readonly StringBuilder _text = new ();

        public string Text { get => _text.ToString(); }

        public static readonly string[] SupportedTags = [ "p", "div" ];

        public override void AddText(string text) => _text.Append(text);

        public override void Draw()
        {
            if (this.Dimensions == Vector2i.Zero) this.Dimensions = TextRenderer.MeasureText(this.Text);

            this.Renderer.AddCoordinates(this.Container.GetCoords(this.Depth));

            string id = Convert.ToString(this.GetHashCode());

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


            if (!this.Renderer.Textures.ContainsKey(id) && this.Renderer.Coords is not null)
            {
                int renderedText = TextRenderer.CreateText(this, ref this.Renderer.Coords);
                this.Renderer.AddTexture(id, renderedText);
            }

            if (!this.Renderer.IsBuffersSet)
            {
                GraphicsBuffer vbo = new (BufferTarget.ArrayBuffer);
                GraphicsBuffer ebo = new (BufferTarget.ElementArrayBuffer);
                
                this.Renderer.AddVertexBufferObject(vbo);
                this.Renderer.AddElementBufferObject(ebo);
            }

            this.Renderer.UseShader();
            var matrix = Shader.Orthogonal;
            int loc = Shader.GetUniformLocation(this.Renderer.Shader, "transform");
            if (loc != -1) GL.UniformMatrix4(loc, true, ref matrix);

            this.Renderer.BindVao();
            this.Renderer.BindBuffer(BufferTarget.ArrayBuffer);
            this.Renderer.BindBuffer(BufferTarget.ElementArrayBuffer);

            this.Renderer.AddAttributePointer(0, 3, VertexAttribPointerType.Float, 5 * sizeof(float));
            this.Renderer.BindTexture(id);
            this.Renderer.AddAttributePointer(1, 2, VertexAttribPointerType.Float, 5 * sizeof(float), 3 * sizeof(float));

            GL.DrawElements(BeginMode.Triangles, GraphicsBuffer.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}