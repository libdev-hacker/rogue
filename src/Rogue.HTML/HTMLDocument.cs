using System.Xml;
using System.Collections;

namespace Rogue.HTML
{
    public class HTMLDocument: IEnumerable<HTMLElement>
    {
        public HTMLElement Root { get; private set; }

        private readonly XmlTextReader _reader;

        private HTMLElement _current = new ();

        public HTMLDocument(string html)
        {
            this.Root = _current;

            using (StringReader stringReader = new (html))
            {
                _reader = new (stringReader);

                while (_reader.Read())
                {
                    switch (_reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            ParseElement();
                            break;
                        case XmlNodeType.Text:
                            if (_current is HTMLTextElement) _current.AddText(_reader.Value);
                            break;
                        case XmlNodeType.EndElement:
                            if (!_current.IsRoot) _current = _current.Parent ?? new (); // Annoying
                            break;
                    }
                }
                _reader.Dispose();
            }
        }

        private void ParseElement()
        {
            string name = _reader.Name;
            HTMLElement element = HTMLTextElement.SupportedTags.Contains(name) ? new HTMLTextElement() : new ();
            element.PopulateAttributes(_reader);
            element.TagName = name;

            _current.AddChild(element);
            element.Parent = _current;
            _current = element;
        }

        public IEnumerator<HTMLElement> GetEnumerator()
        {
            Queue<HTMLElement> elements = new ();
            elements.Enqueue(this.Root);

            while (elements.Any())
            {
                HTMLElement current = elements.Dequeue();
                current.Children.ForEach(elements.Enqueue);
                yield return current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}