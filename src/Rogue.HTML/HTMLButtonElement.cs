using SixLabors.ImageSharp;

using Rogue.Graphics;
using Rogue.JS;
using Rogue.Utils;

namespace Rogue.HTML
{
    public class HTMLButtonElement: HTMLElement
    {
        public readonly static string[] SupportedTags = [ "button" ];

        private readonly TextContainer _textContainer = new ();

        public override void AddText(string text)
        {
            _textContainer.AddText(text);
        }

        public override void Draw()
        {
            this.Dimensions = TextRenderer.MeasureText(_textContainer.Text);
            this.Renderer.AddCoordinates(this.Container.GetCoords(this.Depth));

            string id = Convert.ToString(this.GetHashCode());

            if (!this.Renderer.Textures.ContainsKey(id) && this.Renderer.Coords is not null)
            {
                int renderedText = TextRenderer.CreateText(_textContainer.Text, ref this.Renderer.Coords, Color.Grey);
                this.Renderer.AddTexture(id, renderedText);
            }

            this.Renderer.BindTexture(id);

            base.Draw();
        }

        public override void Click(JsEngine? engine = null)
        {
            if (this.Attributes.TryGetValue("onclick", out string? clickMethod))
            {
                engine?.InvokeMethod(clickMethod);
            }
        }
    }
}