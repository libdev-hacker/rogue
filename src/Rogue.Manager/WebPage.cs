using OpenTK.Mathematics;

using Rogue.Graphics;
using Rogue.Graphics.Backends;
using Rogue.HTML;
using Rogue.JS;
using Rogue.JS.DOM;
using Rogue.Utils;

using Veldrid;

namespace Rogue.Manager
{
    public class WebPage
    {
        public string Url { get; }

        private HTMLDocument _htmlDoc = new ();

        private bool _isDocLoaded = false;

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

            if (!_isDocLoaded)
            {
                string? html = url != WebClient.BlankPage ? _client.GetResource("/", null) : File.ReadAllText(blankPagePath);

                if (html == "" || html is null) return; // Temporary way of handling a blank page / bad path

                _htmlDoc = HTMLDocument.ParseDocument(html, _js);
            }

            Framebuffer? fbo = OpenGLResources.MainFrameBuffer;

            using CommandList? commands = _device?.ResourceFactory.CreateCommandList();
            commands?.Begin();

            GraphicsBuffer<uint> indexCpuBuffer = GraphicsBuffer.Indices;
            DeviceBuffer? indexBuffer = _device?.ResourceFactory.CreateBuffer(indexCpuBuffer.Describe());
            if (indexBuffer is not null)
            {
                _device?.UpdateBuffer(indexBuffer, indexCpuBuffer.GetByteOffset(0), indexCpuBuffer.BufferData);
                commands?.SetIndexBuffer(indexBuffer, IndexFormat.UInt32);
            }


            commands?.SetFramebuffer(fbo!);
            commands?.ClearColorTarget(0, RgbaFloat.White);

            foreach (HTMLElement element in _htmlDoc)
            {
                element.Draw();
            }

            commands?.End();

            _device?.SubmitCommands(commands!);
            _device?.SwapBuffers();
            _device?.WaitForIdle();
        }

        public void RegisterClick(Vector2i clickPoint)
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