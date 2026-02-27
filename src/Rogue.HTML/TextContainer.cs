using System.Text;

namespace Rogue.HTML
{
    public class TextContainer
    {
        public string Text { get => _text.ToString(); set => _text = new (value); }

        private StringBuilder _text = new ();

        public void AddText(string addedText) => _text.Append(addedText);

        public void operator +=(string textToAdd) => this.AddText(textToAdd);

    }
}