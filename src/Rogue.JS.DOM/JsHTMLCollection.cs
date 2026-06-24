using Jint;
using Jint.Native;

namespace Rogue.JS.DOM
{
    public class JsHTMLCollection
    {
        public int Length { get => _elements.Count(); }

        private readonly IEnumerable<JsElement> _elements;

        private readonly JsEngine _engine;

        public JsHTMLCollection(IEnumerable<JsElement> elements, JsEngine engine)
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