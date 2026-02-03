using System.Text;

using OpenTK.Graphics.OpenGL4;
using Rogue.Graphics;
using Rogue.Utils;
using GraphicsBuffer = Rogue.Graphics.Buffer;

namespace Rogue.HTML
{
    public class HTMLTextElement: HTMLElement
    {
        private StringBuilder _text = new ();

        public string Text { get => _text.ToString(); }

        public static readonly string[] SupportedTags = [ "p", "div" ];

        public override void AddText(string text) => _text.Append(text);

        public override void Draw()
        {
            float[] vertices = this.Container.GetCoords(this.Depth);

            string id = Convert.ToString(this.GetHashCode());

            bool hasTexture = false;

            if (this.Renderer.Textures is not null)
            {
                if (!this.Renderer.Textures.ContainsKey(id))
                {
                    int renderedText = TextRenderer.CreateText(this, ref vertices);
                    this.Renderer.AddTexture($"{this.GetHashCode()}", renderedText);
                    hasTexture = true;
                } else
                {
                    hasTexture = true;
                }
            }


            if (!this.Renderer.IsBuffersSet)
            {
                GraphicsBuffer vbo = new (vertices, BufferTarget.ArrayBuffer, BufferUsageHint.DynamicDraw);
                GraphicsBuffer ebo = new (BufferUsageHint.StaticDraw);
                
                this.Renderer.AddVertexBufferObject(vbo);
                this.Renderer.AddElementBufferObject(ebo);
            }


            this.Renderer.BindBuffer(BufferTarget.ArrayBuffer);
            this.Renderer.BindVao();
            this.Renderer.BindBuffer(BufferTarget.ElementArrayBuffer);

            this.Renderer.AddAttributePointer(0, 3, VertexAttribPointerType.Float, 5 * sizeof(float));
            if (hasTexture)
            {
                int textureLocation = Shader.GetAttributeLocation(this.Renderer.Shader, "texCoords");
                this.Renderer.AddAttributePointer(textureLocation, 2, VertexAttribPointerType.Float, 5 * sizeof(float), 3 * sizeof(float));
            }

            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }
    }
}