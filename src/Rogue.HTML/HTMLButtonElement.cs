using SixLabors.ImageSharp;

using Rogue.Graphics;
using Rogue.JS;
using Rogue.Utils;

namespace Rogue.HTML
{
    public class HTMLButtonElement: HTMLElement, ITags
    {
        public static string[] SupportedTags { get; } = [ "button" ];

        private readonly TextContainer _textContainer = new ();

        public override void AddText(string text)
        {
            _textContainer.AddText(text);
        }

        public override void Draw()
        {
        }

        public override void Click(JsEngine? engine = null)
        {
            if (this.Attributes.TryGetValue("onclick", out string? clickMethod))
            {
                engine?.RunScript(clickMethod);
            }
        }
    }
}