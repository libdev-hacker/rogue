using OpenTK.Graphics.OpenGL4;

namespace Rogue.Graphics
{
    public class DrawingContext: IDisposable
    {
        public IList<int>? Textures { get => _textures?.AsReadOnly(); }

        public int Shader { get; private set; }

        private List<int>? _textures;

        private bool _disposed;

        ~DrawingContext() => Dispose(false);

        public void AddTexture(int handle)
        {
            if (GL.IsTexture(handle))
            {
                _textures ??= [];
                _textures.Add(handle);
            }
        }

        public void AddTexture(int handle, int index)
        {
            if (GL.IsTexture(handle))
            {
                if (_textures is not null)
                {
                    _textures[index] = handle;
                }
            }
        }

        public void AddShader(int handle)
        {
            if (GL.IsShader(handle))
            {
                this.Shader = handle;
            }
        }

        public void UseShader()
        {
            if (GL.IsShader(this.Shader))
            {
                GL.UseProgram(this.Shader);
            }
        }

        public void BindTexture(int index)
        {
            if (_textures is not null)
            {
                GL.BindTexture(TextureTarget.Texture2D, _textures[index]);
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
                        foreach (int texture in _textures)
                        {
                            GL.DeleteTexture(texture);
                        }
                    }

                    _textures = null;

                    GL.DeleteProgram(this.Shader);

                    _disposed = true;
                }
            }
        }
    }
}