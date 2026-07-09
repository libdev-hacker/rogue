using System.Collections;

using Veldrid;

using Rogue.Graphics.Backends;

namespace Rogue.Manager
{
    public class TabManager: IEnumerable<WebPage>
    {
        public GraphicsDevice? Graphics;

        public LinkedListNode<WebPage> Current { get; private set; }

        private LinkedList<WebPage> _webpages = new ();

        public TabManager()
        {
            WebPage newPage = new ();
            _webpages.AddFirst(newPage);
            
            this.Current = newPage;
        }

        public TabManager(in OpenGLInfo graphicsOpts): this()
        {
            this.Graphics = GraphicsDevice.CreateOpenGL(graphicsOpts.Opts, graphicsOpts.Info, graphicsOpts.Width, graphicsOpts.Height);
        }

        public void CreateTab(string url, bool switchTabs = false)
        {
            WebPage newPage = new (url);
            _webpages.AddLast(newPage);
            if (switchTabs) SwitchTab(newPage);
        }

        public void SwitchTab(WebPage desiredPage) => this.Current = desiredPage;

        public void DeleteTab(WebPage pageToDelete)
        {
            WebPage current = (WebPage) this.Current;
            if (current == pageToDelete)
            {
                this.Current = this.Current.Previous ?? new WebPage();
            }
            
            _webpages.Remove(pageToDelete);
        }

        public IEnumerator<WebPage> GetEnumerator() => _webpages.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}