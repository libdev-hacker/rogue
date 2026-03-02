using System.Xml;
using System.Collections;

namespace Rogue.HTML
{
    public class HTMLDocument: IEnumerable<HTMLElement>
    {
        public HTMLElement? Root { get; private set; }

        public bool Loaded { get; private set; }

        private XmlReader? _reader;

        private HTMLElement _current = new ();

        public void ParseDocument(string html)
        {
            using (StringReader stringReader = new (html))
            {
                using (_reader ??= XmlReader.Create(stringReader))
                {
                    try
                    {
                        while (_reader.Read())
                        {
                            switch (_reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    ParseElement();
                                    break;
                                case XmlNodeType.Text:
                                    _current.AddText(_reader.Value);
                                    break;
                                case XmlNodeType.EndElement:
                                    if (!_current.IsRoot) _current = _current.Parent ?? new (); // Annoying
                                    break;
                            }
                        }
                    }
                    catch (XmlException e)
                    {
                        if (_reader.EOF && !e.Message.Contains("closed")) throw;
                    }
                }
            }
            this.Loaded = true;
        }

        public HTMLElement[] SearchTree(string property, PropertyType propertyType)
        {
            HTMLElement[] foundElements = [];

            switch (propertyType)
            {
                case PropertyType.Class:
                    foundElements = this.Where(x =>
                    {
                        if (x.Attributes.TryGetValue("class", out string? className))
                        {
                            return className == property;
                        }
                        return false;
                    }).ToArray();
                    break;
                case PropertyType.Id:
                    foundElements = [this.First(x =>
                    {
                        if (x.Attributes.TryGetValue("id", out string? id))
                        {
                            return id == property;
                        }
                        return false;
                    })];
                    break;
                case PropertyType.TagName:
                    foundElements = this.Where(x => x.TagName == property).ToArray();
                    break;
            }

            return foundElements;
        }

        private void ParseElement()
        {
            if (_reader is not null)
            {
                string name = _reader.Name;
                HTMLElement element = GetElementType(name);
                element.PopulateAttributes(_reader);
                element.TagName = name;

                if (_current.TagName == "")
                {
                    _current = element;
                    this.Root ??= _current;
                } else
                {
                    _current.AddChild(element);
                    element.Parent = _current;
                    if (element.HasEndTag) _current = element;
                }
            }
        }

        public IEnumerator<HTMLElement> GetEnumerator()
        {
            Queue<HTMLElement> elements = new ();
            
            if (this.Root is null)
            {
                yield break;
            } else
            {
                elements.Enqueue(this.Root);
            }

            while (elements.Any())
            {
                HTMLElement current = elements.Dequeue();
                current.Children.ForEach(elements.Enqueue);
                yield return current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static HTMLElement GetElementType(string tagName)
        {
            if (HTMLTextElement.SupportedTags.Contains(tagName))
            {
                return new HTMLTextElement();
            } else if (HTMLImageElement.SupportedTags.Contains(tagName))
            {
                return new HTMLImageElement();
            } else
            {
                return new HTMLElement();
            }
        }

    }
}