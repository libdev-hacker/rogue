using System.Text;

namespace Rogue.HTML
{
    public class HTMLTextElement: HTMLElement
    {
        private StringBuilder _text = new ();

        public string Text { get => _text.ToString(); }

        public static readonly string[] SupportedTags = [ "p", "div" ];

        public override void AddText(string text) => _text.Append(text);
    }
}