using Jint.Native;

using Rogue.HTML;
// using Rogue.JS;

namespace Rogue.JS.DOM
{
    public class JsElement
    {
        public string Id { get; private set; }

        public string TagName { get; private set; }

        public int ChildElementCount { get => _element.Children.Count; }

        private readonly HTMLElement _element;

        private JsElement(HTMLElement element)
        {
            this.Id = element.Attributes.TryGetValue("id", out string? id) ? id : "";
            this.TagName = element.TagName;
            
            _element = element;
        }

        public string? GetAttribute(string name)
        {
            _element.Attributes.TryGetValue(name, out string? value);
            return value;
        }

        public static implicit operator JsElement(HTMLElement element) => new (element);

        public JsValue ToJsValue(JsEngine engine) => JsValue.FromObject(engine.Engine, this);
    }
}