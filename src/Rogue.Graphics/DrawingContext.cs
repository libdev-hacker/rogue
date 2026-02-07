using OpenTK.Graphics.OpenGL4;

namespace Rogue.Graphics
{
    public class DrawingContext: IDisposable
    {
        public int Shader { get; private set; }

        public IDictionary<string, int> Textures { get => _textures.AsReadOnly(); }
        
        public int Vbo { get; private set; }
        
        public int? Ebo { get; private set; }

        public bool IsBuffersSet { get => !(this.Vbo == default || this.Ebo == default); }

        public readonly int Vao = GL.GenVertexArray();

        private Dictionary<string, int> _textures = [];

        private bool _disposed;

        ~DrawingContext() => Dispose(false);

        public void AddTexture(string name, int handle)
        {
            if (GL.IsTexture(handle))
            {
                _textures.Add(name, handle);
            }
        }

        public void AddShader(int handle)
        {
            if (GL.IsShader(handle))
            {
                this.Shader = handle;
            }
        }

        public void AddVertexBufferObject(Buffer buffer)
        {
            if (GL.IsBuffer(buffer.Handle) && buffer.BufferType == BufferTarget.ArrayBuffer)
            {
                this.Vbo = buffer.Handle;
            }
        }

        public void AddElementBufferObject(Buffer buffer)
        {
            if (GL.IsBuffer(buffer.Handle) && buffer.BufferType == BufferTarget.ElementArrayBuffer)
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
            if (GL.IsShader(this.Shader))
            {
                GL.UseProgram(this.Shader);
            }
        }

        public void BindTexture(string name)
        {
            if (_textures is not null)
            {
                GL.BindTexture(TextureTarget.Texture2D, _textures[name]);
            }
        }

        public void BindBuffer(BufferTarget target)
        {
            if (target == BufferTarget.ArrayBuffer)
            {
                GL.BindBuffer(target, this.Vbo);
            } else if (target == BufferTarget.ElementArrayBuffer)
            {
                if (this.Ebo is not null) GL.BindBuffer(target, (int)this.Ebo); // More annoying
            }
        }

        public void BindVao()
        {
            if (GL.IsVertexArray(this.Vao))
            {
                GL.BindVertexArray(this.Vao);
            }
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

                    _disposed = true;
                }
            }
        }
    }
}