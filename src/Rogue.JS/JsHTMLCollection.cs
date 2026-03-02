using Jint;
using Jint.Native;

namespace Rogue.JS
{
    public class JsHTMLCollection
    {
        public int Length { get => _elements.Count(); }

        private readonly IEnumerable<JsElement> _elements;

        private readonly Engine _engine;

        public JsHTMLCollection(IEnumerable<JsElement> elements, Engine engine)
        {
            _elements = elements;
            _engine = engine;
        }

        public JsValue this[int index]
        {
            get
            {
                JsElement? element = this.Items(index);
                if (element is null) return JsValue.Undefined;
                return element.ToJsValue(_engine);
            }
        }

        public JsElement? Items(int index)
        {
            if (index < 0 || index > this.Length)
            {
                return null;
            }

            return _elements.ElementAt(index);
        }
    }
}