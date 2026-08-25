
using Veldrid;

using Rogue.Graphics.Backends;
using Rogue.Graphics.Text;

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Rogue.Graphics
{
    public class DrawingContext: IDisposable
    {
        public ShaderProgram? Shader { get; private set; }

        public DeviceBuffer? VertexBuffer { get; private set; }

        public CommandList Commands { get; }

        public Texture[] Textures { get => [.. _textures.Values]; }

        public List<ResourceLayoutElementDescription> LayoutElements { get; } = [];

        public List<BindableResource> Resources { get; } = [];

        public List<VertexElementDescription> VertexLayout { get; } = [
            new VertexElementDescription(
                ShaderProgram.CoordinateName,
                VertexElementFormat.Float3,
                VertexElementSemantic.Position
            )
        ];

        private Dictionary<string, Texture> _textures = [];

        private GraphicsDevice _device = OpenGLResources.Device ?? throw new Exception("GraphicsDevice not instantiated yet!"); // Easy alias

        private CharacterAtlas _charAtlas = CharacterLoader.LoadDefaultFont();

        private bool _disposed;

        public DrawingContext()
        {
            this.Commands = _device.ResourceFactory.CreateCommandList();
            this.Commands.Begin();
        }

        ~DrawingContext() => Dispose(false);

        public void SetCoordinates(float[] coords)
        {
            DeviceBuffer buffer = this.LoadBuffer(coords, BufferUsage.VertexBuffer);
            
            this.Commands.SetVertexBuffer(0, buffer);
            this.VertexBuffer = buffer;
        }

        public void AddShaders(string vertexShader, string fragShader) => this.Shader ??= new (vertexShader, fragShader, _device.ResourceFactory);

        public void AddUniform<T>(string name, T[] data, ShaderStages stage) where T: unmanaged
        {
            DeviceBuffer nativeBuffer = this.LoadBuffer(data, BufferUsage.UniformBuffer);

            this.LayoutElements.Add(new ResourceLayoutElementDescription(
                name,
                ResourceKind.UniformBuffer,
                stage
            ));

            this.Resources.Add(nativeBuffer);
        }

        public void AddTexture(Texture texture) => _textures.Add(texture.Name!, texture);

        public void BindTexture(string textureName, string resourceName = "texture")
        {
            if (!this.VertexLayout.Any(x => x.Name == ShaderProgram.TextureCoordName))
            {
                this.VertexLayout.Add(new (
                    ShaderProgram.TextureCoordName,
                    VertexElementFormat.Float2,
                    VertexElementSemantic.TextureCoordinate
                ));
            }

            Texture selectedTexture = _textures[textureName];
            TextureView view = _device.ResourceFactory.CreateTextureView(selectedTexture);

            this.LayoutElements.Add(new (resourceName, ResourceKind.TextureReadOnly, ShaderStages.Fragment));
            this.Resources.Add(view);
        }

        private void BindTexture(TextureView texture, string resourceName = "texture")
        {
            if (!this.VertexLayout.Any(x => x.Name == ShaderProgram.TextureCoordName))
            {
                this.VertexLayout.Add(new (
                    ShaderProgram.TextureCoordName,
                    VertexElementFormat.Float2,
                    VertexElementSemantic.TextureCoordinate
                ));
            }

            this.LayoutElements.Add(new (resourceName, ResourceKind.TextureReadOnly, ShaderStages.Fragment));
            this.Resources.Add(texture);
        }

        public void AddCharacterAtlas() => this.BindTexture(_charAtlas.Texture, "atlas");

        public void AddCharacterAtlas(Font font)
        {
            _charAtlas = CharacterLoader.LoadAsciiFromFont(font);
            this.AddCharacterAtlas();
        }

        private Pipeline SetupPipeline()
        {
            GraphicsPipelineDescription pipeline = OpenGLResources.CreatePipeline();
            pipeline.Outputs = OpenGLResources.MainFrameBuffer?.OutputDescription ?? throw new Exception("No SwapchainFramebuffer found");

            ResourceLayoutDescription layoutDescription = new ([.. this.LayoutElements]);
            pipeline.ResourceLayouts = [_device.ResourceFactory.CreateResourceLayout(layoutDescription)];

            ShaderSetDescription shaders = new ([new VertexLayoutDescription(this.VertexLayout.ToArray())], this.Shader!.ToArray());
            pipeline.ShaderSet = shaders;

            return _device.ResourceFactory.CreateGraphicsPipeline(pipeline);
        }

        private void SetIndices()
        {
            GraphicsBuffer<uint> indexCpuBuffer = GraphicsBuffer.Indices;
            DeviceBuffer indexBuffer = _device.ResourceFactory.CreateBuffer(indexCpuBuffer.Describe());

            _device.UpdateBuffer(indexBuffer, indexCpuBuffer.GetByteOffset(0), indexCpuBuffer.BufferData);
            this.Commands.SetIndexBuffer(indexBuffer, IndexFormat.UInt32);
        }

        public void DrawElement()
        {
            if (this.VertexBuffer is not null)
            {
                this.Commands.SetPipeline(this.SetupPipeline());

                ResourceLayout layout = _device.ResourceFactory.CreateResourceLayout(new ([.. this.LayoutElements]));
                ResourceSet set = _device.ResourceFactory.CreateResourceSet(new (layout, [.. this.Resources]));
                this.Commands.SetGraphicsResourceSet(0, set);

                this.Commands.Draw(this.VertexBuffer.SizeInBytes / sizeof(float));
            }
        }

        private DeviceBuffer LoadBuffer<T>(T[] bufferData, BufferUsage usage, int offset = 0) where T: unmanaged
        {
            GraphicsBuffer<T> buffer = new (bufferData, usage);

            DeviceBuffer nativeBuffer = _device.ResourceFactory.CreateBuffer(buffer.Describe());
            _device.UpdateBuffer(nativeBuffer, buffer.GetByteOffset(offset), bufferData);

            return nativeBuffer;
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

                foreach (Texture texture in this.Textures)
                {
                    texture.Dispose();
                }

                foreach (Shader shader in this.Shader!.ToArray())
                {
                    shader.Dispose();
                }
            }

            _disposed = true;
        }
    }
}