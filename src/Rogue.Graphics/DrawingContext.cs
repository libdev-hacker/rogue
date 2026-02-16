using OpenTK.Graphics.OpenGL4;

namespace Rogue.Graphics
{
    public class DrawingContext: IDisposable
    {
        public int Shader { get; private set; } = -1;

        public IDictionary<string, int> Textures { get => _textures.AsReadOnly(); }
        
        public int Vbo { get; private set; }
        
        public int? Ebo { get; private set; }

        public ref float[]? Coords { get => ref _coords; }

        public bool IsBuffersSet { get => !(this.Vbo == default || this.Ebo == default); }

        public readonly int Vao = GL.GenVertexArray();

        private Dictionary<string, int> _textures = [];

        private float[]? _coords;

        private bool _disposed;

        ~DrawingContext() => Dispose(false);

        public void AddTexture(string name, int handle)
        {
            _textures.Add(name, handle);
        }

        public void AddShader(int handle)
        {
            this.Shader = handle;
        }

        public void AddCoordinates(float[] coords)
        {
            _coords ??= coords;
        }

        public void AddVertexBufferObject(Buffer buffer)
        {
            if (buffer.BufferType == BufferTarget.ArrayBuffer)
            {
                this.Vbo = buffer.Handle;
            }
        }

        public void AddElementBufferObject(Buffer buffer)
        {
            if (buffer.BufferType == BufferTarget.ElementArrayBuffer)
            {
                this.Ebo ??= buffer.Handle;
            }
        }

        public void AddAttributePointer(int index, int count, VertexAttribPointerType type, int stride, int offset = 0)
        {
            GL.VertexAttribPointer(index, count, type, false, stride, offset);
            GL.EnableVertexAttribArray(index);
        }

        public void UseShader()
        {
           GL.UseProgram(this.Shader);
        }

        public void BindTexture(string name)
        {
            if (_textures is not null)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, _textures[name]);
            }
        }

        public void BindBuffer(BufferTarget target)
        {
            if (target == BufferTarget.ArrayBuffer && _coords is not null)
            {
                GL.BindBuffer(target, this.Vbo);
                GL.BufferData(target, _coords.Length * sizeof(float), _coords, BufferUsageHint.DynamicDraw);
            } else if (target == BufferTarget.ElementArrayBuffer)
            {
                if (this.Ebo is not null)
                {
                    GL.BindBuffer(target, (int)this.Ebo); // More annoying
                    GL.BufferData(target, Buffer.Indices.Length * sizeof(uint), Buffer.Indices, BufferUsageHint.StaticDraw);
                }
            }
        }

        public void BindVao()
        {
            GL.BindVertexArray(this.Vao);
        }

        public void Dispose()
        {
            // Disposing of object
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Disposing of OpenGL resources
                    GL.Finish();
                    GL.DeleteBuffer(this.Vao);

                    if (_textures is not null)
                    {
                        foreach (int texture in _textures.Values)
                        {
                            GL.DeleteTexture(texture);
                        }
                    }

                    _textures?.Clear();

                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                    GL.DeleteBuffer(this.Vbo);

                    this.Vbo = 0;

                    if (this.Ebo is not null)
                    {
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                        GL.DeleteBuffer((int)this.Ebo);
                        this.Ebo = 0;
                    }

                    GL.DeleteProgram(this.Shader);
                    this.Shader = 0;

                    _coords = null;

                    _disposed = true;
                }
            }
        }
    }
}