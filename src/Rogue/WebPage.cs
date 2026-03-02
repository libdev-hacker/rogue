using Rogue.HTML;
using Rogue.JS;
using Rogue.Utils;

namespace Rogue
{
    public class WebPage
    {
        public string Url { get; }

        private HTMLDocument _htmlDoc = new ();

        private WebClient _client;

        private string? _html;

        private JsEngine _js = new ();

        private JsDocument _jsDocument;

        public WebPage(string url = "")
        {
            this.Url = url;
            _client = new (url);

            _jsDocument = new(_htmlDoc, _js.Engine)
            {
                URL = this.Url
            };
            
            this.PrepareJsEngine();
        }

        public void RenderPage()
        {
            if (_client.Uri.AbsoluteUri != WebClient.BlankPage)
            {
                _html ??= _client.GetResource("/", null);

                if (_html is null) return; // Temporary way of handling a blank page / bad path

                if (!_htmlDoc.Loaded) _htmlDoc.ParseDocument(_html, _js);

                foreach (HTMLElement element in _htmlDoc)
                {
                    element.Draw();
                }
            }
        }

        public void CleanUp()
        {
            if (_htmlDoc is not null)
            {
                foreach (HTMLElement element in _htmlDoc)
                {
                    element.EndDraw();
                }
            }
        }

        private void PrepareJsEngine()
        {
            _js.AddNativeObject(new Action<object>(Console.WriteLine), "log");
            _js.AddNativeObject(_jsDocument, "document");

            _js.AddNativeClass<JsElement>("Element");
            _js.AddNativeClass<JsHTMLCollection>("HTMLCollection");
        }

        public static implicit operator LinkedListNode<WebPage>(WebPage page) => new (page);
    }
}