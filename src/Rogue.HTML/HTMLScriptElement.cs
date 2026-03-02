using Rogue.JS;

namespace Rogue.HTML
{
    public class HTMLScriptElement: HTMLElement
    {
        public static readonly string[] SupportedTags = [ "script" ];
        
        private TextContainer _text = new ();

        public override void AddText(string text) => _text.AddText(text);

        public void RunScript(JsEngine engine) => engine.Engine.Execute(_text.Text);
    }
}