

namespace Rogue
{
    public class TabManager
    {

        public LinkedListNode<WebPage> Current { get; private set; }

        private LinkedList<WebPage> _webpages;

        public TabManager()
        {
            _webpages = new ();
            WebPage newPage = new ();
            _webpages.AddFirst(newPage);
            
            this.Current = newPage;
        }

        public void CreateTab(string url)
        {
            WebPage newPage = new (url);
            _webpages.AddLast(newPage);
            SwitchTab(newPage);
        }

        public void SwitchTab(WebPage desiredPage) => this.Current = desiredPage;

        public void DeleteTab(WebPage pageToDelete)
        {

            if (this.Current.Value == pageToDelete)
            {
                this.Current = this.Current.Previous ?? new WebPage();
            }
            
            _webpages.Remove(pageToDelete);
        }
    }
}