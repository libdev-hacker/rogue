using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

using OpenTK.Mathematics;

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

            this.Renderer.BindTexture(id);

            base.Draw();
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