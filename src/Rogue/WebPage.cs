using Rogue.Utils;

namespace Rogue
{
    public class WebPage
    {
        public string Url { get; init; }

        private WebClient _client;

        public WebPage(string url = "")
        {
            this.Url = url;
            _client = new (url);
        }

        public static implicit operator LinkedListNode<WebPage>(WebPage page) => new (page);
    }
}