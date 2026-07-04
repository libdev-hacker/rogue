using Rogue.JS;

namespace Rogue.HTML
{
    public class HTMLScriptElement: HTMLElement, ITags
    {
        public static string[] SupportedTags { get; } = [ "script" ];
        
        private TextContainer _text = new ();

        public override void AddText(string text) => _text.AddText(text);

        public void RunScript(JsEngine engine) => engine.RunScript(_text.Text);
    }
}