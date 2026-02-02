using OpenTK.Graphics.OpenGL4;

namespace Rogue.Graphics
{
    public class DrawingContext: IDisposable
    {
        private int _shader;

        private Dictionary<string, int>? _textures;

        private bool _disposed;

        ~DrawingContext() => Dispose(false);

        public void AddTexture(string name, int handle)
        {
            if (GL.IsTexture(handle))
            {
                _textures ??= [];
                _textures.Add(name, handle);
            }
        }

        public void AddShader(int handle)
        {
            if (GL.IsShader(handle))
            {
                _shader = handle;
            }
        }

        public void UseShader()
        {
            if (GL.IsShader(_shader))
            {
                GL.UseProgram(_shader);
            }
        }

        public void BindTexture(string name)
        {
            if (_textures is not null)
            {
                GL.BindTexture(TextureTarget.Texture2D, _textures[name]);
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

                    _textures = null;

                    GL.DeleteProgram(_shader);

                    _disposed = true;
                }
            }
        }
    }
}