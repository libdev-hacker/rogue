using Rogue.Graphics;
using Rogue.Utils;

namespace Rogue.HTML
{
    public class HTMLTextElement: HTMLElement
    {
        public static readonly string[] SupportedTags = [ "p", "div" ];

        public TextContainer InnerText { get; } = new ();
        
        public override void AddText(string text) => this.InnerText.AddText(text);

        public override void Draw()
        {
            this.Dimensions = TextRenderer.MeasureText(this.InnerText.Text);

            this.Renderer.AddCoordinates(this.Container.GetCoords(this.Depth));

            string id = Convert.ToString(this.GetHashCode());

            if (!this.Renderer.Textures.ContainsKey(id) && this.Renderer.Coords is not null)
            {
                int renderedText = TextRenderer.CreateText(this.InnerText.Text, ref this.Renderer.Coords);
                this.Renderer.AddTexture(id, renderedText);
            }

            this.Renderer.BindTexture(id);

            base.Draw();
        }
    }
}