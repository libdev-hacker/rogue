using System.Xml;
using System.Collections;

using Rogue.JS;

namespace Rogue.HTML
{
    public class HTMLDocument: IEnumerable<HTMLElement>
    {
        public HTMLElement? Root { get; private set; }

        public string Content { get; } = "";

        public HTMLDocument() {}

        private HTMLDocument(string html) => this.Content = html;

        public static HTMLDocument ParseDocument(string html, JsEngine engine)
        {
            HTMLDocument document = new (html);
            HTMLElement current = new ();

            using (StringReader stringReader = new (html))
            {
                using (XmlTextReader reader = new (stringReader))
                {
                    try
                    {
                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    document.ParseElement(reader, current);
                                    break;
                                case XmlNodeType.Text:
                                    current.AddText(reader.Value);
                                    break;
                                case XmlNodeType.EndElement:
                                    if (current is HTMLScriptElement script) script.RunScript(engine);
                                    if (!current.IsRoot) current = current.Parent;
                                    reader.ResetState();
                                    break;
                            }
                        }
                    }
                    catch (XmlException e)
                    {
                        if (reader.EOF && !e.Message.Contains("closed")) throw;
                    }
                }
            }
            return document;
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

        internal void ParseElement(XmlTextReader reader, HTMLElement current)
        {
            if (reader is not null)
            {
                string name = reader.Name;
                HTMLElement element = GetElementType(name);
                element.PopulateAttributes(reader);
                element.TagName = name;

                if (current.TagName == "")
                {
                    current = element;
                    this.Root ??= current;
                } else
                {
                    current.AddChild(element);
                    element.Parent = current;
                    if (element.HasEndTag) current = element;
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
            } else if (HTMLScriptElement.SupportedTags.Contains(tagName))
            {
                return new HTMLScriptElement();
            } else if (HTMLButtonElement.SupportedTags.Contains(tagName))
            {
                return new HTMLButtonElement();
            }

            return new HTMLElement();
        }

    }
}
