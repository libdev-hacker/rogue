using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

using Rogue.Graphics;
using Rogue.Utils;

namespace Rogue.HTML
{
    public class HTMLImageElement: HTMLElement, ITags
    {
        public static string[] SupportedTags { get; } = [ "img" ];

        private Image<Rgba32>? _image;

        private TextContainer? _altText;

        public HTMLImageElement() => this.HasEndTag = false;

        public override void AddText(string text)
        {
            if (_altText is not null) _altText += text;
        }

        public override void Draw()
        {
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