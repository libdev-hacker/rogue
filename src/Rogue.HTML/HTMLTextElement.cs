using Rogue.Graphics;
using Rogue.Graphics.Text;
using Rogue.Utils;

namespace Rogue.HTML
{
    public class HTMLTextElement: HTMLElement, ITags
    {
        public static string[] SupportedTags { get; } = [ "p", "div" ];

        public TextContainer InnerText { get; } = new ();
        
        public override void AddText(string text) => this.InnerText.AddText(text);

        public override void Draw()
        {
        }
    }
}