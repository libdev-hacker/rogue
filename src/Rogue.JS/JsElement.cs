using Jint;
using Jint.Native;

using Rogue.HTML;

namespace Rogue.JS
{
    public class JsElement
    {
        public string Id { get; private set; }

        public string TagName { get; private set; }

        public int ChildElementCount { get; private set; }

        private readonly HTMLElement _element;

        private JsElement(HTMLElement element)
        {
            this.Id = element.Attributes.TryGetValue("id", out string? id) ? id : "";
            this.TagName = element.TagName;
            this.ChildElementCount = element.Children.Count;
            
            _element = element;
        }

        public string? GetAttribute(string name)
        {
            _element.Attributes.TryGetValue(name, out string? value);
            return value;
        }

        public static implicit operator JsElement(HTMLElement element) => new (element);

        public JsValue ToJsValue(Engine engine) => JsValue.FromObject(engine, this);
    }
}