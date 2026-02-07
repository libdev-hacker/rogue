using Rogue.HTML;
using Rogue.Utils;

namespace Rogue
{
    public class WebPage
    {
        public string Url { get; }

        private HTMLDocument? _htmlDoc;

        private WebClient _client;

        private string? _html;

        public WebPage(string url = "")
        {
            this.Url = url;
            _client = new (url);
        }

        public void RenderPage()
        {
            if (_client.Uri.AbsoluteUri != "about:blank")
            {
                _html ??= _client.GetResource("/", null);

                if (_html is null) return; // Temporary way of handling a blank page / bad path

                _htmlDoc ??= new (_html);

                foreach (HTMLElement element in _htmlDoc)
                {
                    element.Draw();
                }
            }
        }

        public static implicit operator LinkedListNode<WebPage>(WebPage page) => new (page);
    }
}