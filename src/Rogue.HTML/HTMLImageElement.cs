using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using Rogue.Graphics;
using Rogue.Utils;

namespace Rogue.HTML
{
    public class HTMLImageElement: HTMLElement
    {
        public static readonly string[] SupportedTags = [ "img" ];

        private Image<Rgba32>? _image;

        private TextContainer? _altText;

        public HTMLImageElement() => this.HasEndTag = false;

        public override void AddText(string text)
        {
            if (_altText is not null) _altText += text;
        }

        public override void Draw()
        {
            bool isImage = _image is not null;
            bool isAltText = _altText is not null && _altText.Text != "";

            // Determining what should be rendered
            if (!isImage && !isAltText)
            {
                FetchImage().GetAwaiter().GetResult();
                if (_image is not null)
                {
                    this.Dimensions = new Vector2i(_image.Width, _image.Height);
                } else if (_altText is not null)
                {
                    this.Dimensions = TextRenderer.MeasureText(_altText.Text);
                }
                this.Renderer.AddCoordinates(this.Container.GetCoords(this.Depth));
            }

            // Shader Creation
            if (this.Renderer.Shader == -1)
            {
                const string vertexShader = @"
                    #version 420 core

                    layout(location = 0) in vec3 pos;
                    layout(location = 1) in vec2 texCoords;

                    out vec2 texPoints;

                    uniform mat4 transform;

                    void main() {
                        gl_Position = transform * vec4(pos, 1.0);
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

            // Creating the OpenGL texture
            string id = "";
            if (isImage)
            {
                id = Convert.ToString(this.GetHashCode());
            } else if (isAltText)
            {
                id = _altText!.Text;
            }

            if (!this.Renderer.Textures.ContainsKey(id) && this.Renderer.Coords is not null)
            {
                int texture = 0;
                if (isImage)
                {
                    texture = Texture.CreateTexture(_image!, ref this.Renderer.Coords);
                } else if (isAltText)
                {
                    texture = TextRenderer.CreateText(_altText!.Text, ref this.Renderer.Coords);
                }
                this.Renderer.AddTexture(id, texture);
            }

            if (!this.Renderer.IsBuffersSet)
            {
                GraphicsBuffer vbo = new (BufferTarget.ArrayBuffer);
                GraphicsBuffer ebo = new (BufferTarget.ElementArrayBuffer);
                
                this.Renderer.AddVertexBufferObject(vbo);
                this.Renderer.AddElementBufferObject(ebo);
            }

            this.Renderer.UseShader();
            Shader.SetOrthogonalMatrix(this.Renderer.Shader);

            this.Renderer.BindVao();
            this.Renderer.BindBuffer(BufferTarget.ArrayBuffer);
            this.Renderer.BindBuffer(BufferTarget.ElementArrayBuffer);

            this.Renderer.AddAttributePointer(0, 3, VertexAttribPointerType.Float, 5 * sizeof(float));
            this.Renderer.BindTexture(id);
            this.Renderer.AddAttributePointer(1, 2, VertexAttribPointerType.Float, 5 * sizeof(float), 3 * sizeof(float));

            this.Renderer.DrawElement();
        }

        public new void EndDraw()
        {
            _image?.Dispose();
            base.EndDraw();
        }

        private async Task FetchImage()
        {
            if (this.Attributes.TryGetValue("src", out string? sourceUrl))
            {
                if (sourceUrl is not null)
                {
                    Uri url = new (sourceUrl);
                    WebClient client = new (url.AbsoluteUri.Replace(url.AbsolutePath, ""));
                    Stream? imageFile = await client.GetFile(url.AbsolutePath, null);
                    if (imageFile is not null)
                    {
                        _image ??= await Image.LoadAsync<Rgba32>(imageFile);
                        
                        bool hasWidth = this.Attributes.TryGetValue("width", out string? widthAttribute);
                        bool hasHeight = this.Attributes.TryGetValue("height", out string? heightattribute);
                        int width = Convert.ToInt32(widthAttribute);
                        int height = Convert.ToInt32(heightattribute);

                        if (hasWidth && hasHeight)
                        {
                            _image.Mutate(i => i.Resize(new Size(width, height)));
                        } else if (hasWidth && !hasHeight)
                        {
                            int aspectRatio = _image.Height / _image.Width;
                            _image.Mutate(i => i.Resize(new Size(width, width * aspectRatio)));
                        } else if (!hasWidth && hasHeight)
                        {
                            int aspectRatio = _image.Width / _image.Height;
                            _image.Mutate(i => i.Resize(new Size(height * aspectRatio, height)));
                        }

                        return;
                    }
                }
            }
            
            if (this.Attributes.TryGetValue("alt", out string? altText))
            {
                _altText?.Text = altText;
            }
        }
    }
}