using Jint;

using Rogue.HTML;
using Rogue.JS;

namespace Rogue.JS.DOM
{
    public class JsDocument
    {
        public string URL { get; set; } = "";

        private readonly HTMLDocument _document;

        private readonly JsEngine _engine;

        public JsDocument(HTMLDocument doc, JsEngine engine)
        {
            _document = doc;
            _engine = engine;
        }

        public JsElement? GetAttributeById(string id)
        {
            HTMLElement[] element = _document.SearchTree(id, PropertyType.Id);
            return element.Length == 1 ? element[0] : null!;
        }

        public JsHTMLCollection GetElementsByClassName(string classes)
        {
            string[] classNames = classes.Split(" ");
            List<JsElement> elements = [];

            foreach (string name in classNames)
            {
                HTMLElement[] selectedElements = _document.SearchTree(name, PropertyType.Class);
                foreach (HTMLElement element in selectedElements)
                {
                    elements.Add(element);
                }
            }

            return new (elements, _engine);
        }

        public JsHTMLCollection GetElementsByTagName(string name)
        {
            List<JsElement> elements = [];
            IEnumerable<HTMLElement> selectedElements = name == "*" ? _document : _document.SearchTree(name, PropertyType.TagName);

            foreach (HTMLElement element in selectedElements)
            {
                elements.Add(element);
            }

            return new (elements, _engine);
        }
    }
}