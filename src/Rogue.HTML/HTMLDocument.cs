using System.Xml;

namespace Rogue.HTML
{
    public class HTMLDocument
    {
        public HTMLElement? Root { get; private set; }

        private readonly XmlTextReader _reader;

        private HTMLElement _current = new ();

        public HTMLDocument(string html)
        {
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

            if (this.Root is null)
            {
                this.Root = _current = element;
            } else
            {
                _current.AddChild(element);
                element.Parent = _current;
                _current = element;
            }
        }
    }
}