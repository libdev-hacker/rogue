
using Veldrid;

using Rogue.Graphics.Backends;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using System.Diagnostics.CodeAnalysis;

namespace Rogue.Graphics
{
    public class DrawingContext: IDisposable
    {
        [AllowNull]
        public ShaderProgram Shader { get; private set; }

        public GraphicsBuffer<float> VertexBuffer { get; private set; }

        public CommandList Commands { get; }

        public Texture[] Textures { get => [.. _textures.Values]; }

        public List<ResourceLayoutElementDescription> LayoutElements = [];

        public List<BindableResource> Resources = [];

        private Dictionary<string, Texture> _textures = [];

        private GraphicsDevice _device = OpenGLResources.Device ?? throw new Exception("GraphicsDevice not instantiated yet!"); // Easy alias

        private DeviceBuffer _indexBuffer;

        private bool _disposed;

        public DrawingContext()
        {
            this.Commands = _device.ResourceFactory.CreateCommandList();
            this.Commands.Begin();

            GraphicsBuffer<uint> indexCpuBuffer = GraphicsBuffer.Indices;
            _indexBuffer = _device.ResourceFactory.CreateBuffer(GraphicsBuffer.Indices.Describe());
            _device.UpdateBuffer(_indexBuffer, indexCpuBuffer.GetByteOffset(0), indexCpuBuffer.BufferData);

            this.Commands.SetIndexBuffer(_indexBuffer, IndexFormat.UInt32);
        }

        ~DrawingContext() => Dispose(false);

        public void SetCoordinates(float[] coords)
        {
            GraphicsBuffer<float> vertexBuffer = new (coords, BufferUsage.VertexBuffer);

            DeviceBuffer buffer = _device.ResourceFactory.CreateBuffer(vertexBuffer.Describe());
            _device.UpdateBuffer(buffer, vertexBuffer.GetByteOffset(0), coords);

            this.Commands.SetVertexBuffer(0, buffer);
            this.VertexBuffer = vertexBuffer;
        }

        public void AddShaders(string vertexShader, string fragShader) => this.Shader = new (vertexShader, fragShader, _device.ResourceFactory);

        public void AddTexture(Texture texture) => _textures.Add(texture.Name!, texture);

        public void BindTexture(string name)
        {
            this.LayoutElements.Add(new ("texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment));

            Texture selectedTexture = _textures[name];
            TextureView view = _device.ResourceFactory.CreateTextureView(selectedTexture);
            this.Resources.Add(view);
        }

        public unsafe Image<Rgba32> GetImageFromTexture(string name, bool readOnly = true)
        {
            Texture target = _textures[name];
            MappedResource mappedImage = _device.Map(target, readOnly ? MapMode.Read : MapMode.ReadWrite);

            return Image.WrapMemory<Rgba32>(mappedImage.Data.ToPointer(), (int) mappedImage.SizeInBytes, (int) target.Width, (int) target.Height);
        }

        private Pipeline SetupPipeline()
        {
            GraphicsPipelineDescription pipeline = OpenGLResources.CreatePipeline();
            pipeline.Outputs = _device.SwapchainFramebuffer?.OutputDescription ?? throw new Exception("No SwapchainFramebuffer found");

            ResourceLayoutDescription layoutDescription = new ([.. this.LayoutElements]);
            pipeline.ResourceLayouts = [_device.ResourceFactory.CreateResourceLayout(layoutDescription)];

            ShaderSetDescription shaders = new (null, this.Shader.ToArray());
            pipeline.ShaderSet = shaders;

            return _device.ResourceFactory.CreateGraphicsPipeline(pipeline);
        }

        public static void InitFrame(CommandList commands)
        {
            commands.SetFramebuffer(OpenGLResources.Device?.SwapchainFramebuffer ?? throw new Exception("No SwapChain found"));
            commands.ClearColorTarget(0, RgbaFloat.White);
        }

        public void DrawElement()
        {
            this.Commands.SetPipeline(this.SetupPipeline());

            ResourceLayout layout = _device.ResourceFactory.CreateResourceLayout(new ([.. this.LayoutElements]));
            ResourceSet set = _device.ResourceFactory.CreateResourceSet(new (layout, [.. this.Resources]));
            this.Commands.SetGraphicsResourceSet(0, set);

            this.Commands.Draw((uint) this.VertexBuffer.BufferData.Length);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool dispose)
        {
            if (_disposed) return;

            if (dispose)
            {
                this.Commands.Dispose();

                _indexBuffer.Dispose();
                
                foreach (Texture texture in this.Textures)
                {
                    texture.Dispose();
                }

                foreach (Shader shader in this.Shader.ToArray())
                {
                    shader.Dispose();
                }
            }

            _disposed = true;
        }
    }
}