using Rogue.Graphics.Backends;
using Rogue.HTML;
using Rogue.JS;
using Rogue.JS.DOM;
using Rogue.Utils;
using Rogue.Utils.Maths;

using System.Numerics;

using Veldrid;

namespace Rogue.Manager
{
    public class WebPage
    {
        public string Url { get; }

        private HTMLDocument _htmlDoc = new ();

        private bool _isLoading = false;

        private bool _wait = true;

        private Lock _lock = new ();

        private WebClient _client;

        private JsEngine _js = new ();

        private JsDocument _jsDocument;

        private GraphicsDevice? _device = OpenGLResources.Device;

        public WebPage(string url = "")
        {
            this.Url = url;
            _client = new (url);

            _jsDocument = new(_htmlDoc, _js)
            {
                URL = this.Url
            };
            
            this.PrepareJsEngine();
        }

        public void RenderPage()
        {
            string blankPagePath = Path.GetDirectoryName(Environment.ProcessPath) + "/blank.html";
            string url = _client.Uri.AbsoluteUri;

            if (!_isLoading)
            {
                ThreadPool.QueueUserWorkItem(async (state) =>
                {
                    string? html = url != WebClient.BlankPage ? await _client.GetResourceAsync("/", null).ConfigureAwait(false) : File.ReadAllText(blankPagePath);

                    if (html == "" || html is null) return;

                    lock (_lock)
                    {
                        _htmlDoc = HTMLDocument.ParseDocument(html, _js);
                        _wait = false;
                    }
                });
                _isLoading = true;
            }

            Framebuffer? fbo = OpenGLResources.MainFrameBuffer;

            using CommandList? commands = _device?.ResourceFactory.CreateCommandList();
            commands?.Begin();

            commands?.SetFramebuffer(fbo!);
            commands?.ClearColorTarget(0, RgbaFloat.White);

            lock (_lock)
            {
                if (!_wait && _htmlDoc.Root is not null)
                {
                    foreach (HTMLElement element in _htmlDoc)
                    {
                        element.Draw();
                    }
                }
            }

            commands?.End();

            _device?.SubmitCommands(commands!);
            _device?.WaitForIdle();
        }

        public void RegisterClick(Vector2 clickPoint)
        {
            foreach (HTMLElement element in _htmlDoc)
            {
                if (element.IsPointWithin(clickPoint))
                {
                    element.Click(_js);
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
            _js.AddNativeObject(new JsConsole(), "console");
            _js.AddNativeObject(_jsDocument, "document");

            _js.AddNativeClass<JsElement>("Element");
            _js.AddNativeClass<JsHTMLCollection>("HTMLCollection");
        }

        public static implicit operator LinkedListNode<WebPage>(WebPage page) => new (page);
        public static explicit operator WebPage(LinkedListNode<WebPage> node) => node.Value;
    }
}