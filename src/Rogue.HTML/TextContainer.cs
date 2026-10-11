
using System.Text;

using SixLabors.Fonts;

using Rogue.Graphics.Text;

namespace Rogue.HTML
{
    public class TextContainer
    {
        public string Text { get => _text.ToString(); set => _text = new (value); }

        public TextOptions? FontOptions { get; private set; }

        public TextMetrics Measurements { get => TextMeasurer.Measure(this.Text, this.FontOptions ?? CharacterLoader.DefaultFontOptions); }

        private StringBuilder _text = new ();

        public void AddText(string addedText) => _text.Append(addedText);

        public void operator +=(string textToAdd) => this.AddText(textToAdd);

    }
}