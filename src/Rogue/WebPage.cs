using Rogue.HTML;
using Rogue.Utils;

namespace Rogue
{
    public class WebPage
    {
        public string Url { get; }

        private HTMLDocument? _html;

        private WebClient _client;

        public WebPage(string url = "")
        {
            this.Url = url;
            _client = new (url);
        }

        public void RenderPage()
        {
            if (_client.Uri.AbsoluteUri != "about:blank")
            {
                string? pageContent = _client.GetResource("/", null);

                if (pageContent is null) return; // Temporary way of handling a blank page / bad path

                _html = new (pageContent);

                foreach (HTMLElement element in _html)
                {
                    element.Draw();
                }
            }
        }

        public static implicit operator LinkedListNode<WebPage>(WebPage page) => new (page);
    }
}