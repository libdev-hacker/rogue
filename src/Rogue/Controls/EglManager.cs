using System.Runtime.InteropServices;

using Avalonia.OpenGL;
using Avalonia.OpenGL.Egl;

using Veldrid.OpenGL;

namespace Rogue.Controls
{
    public class EglManager: IDisposable
    {
        private const string _eglLinux = "libEGL-linux.so";

        private const string _eglWin = "libEGL-windows.dll";

        private EglInterface _interface;

        private EglDisplay _display;

        private EglContext _context;

        private EglSurface? _surface;

        private bool _disposed;

        public EglManager()
        {
#if DEBUG
            _interface = new ();
#else
            _interface = new (EglManager.GetLibraryPath());
#endif

            GlVersion[] openglVersion = [new (GlProfileType.OpenGL, 4, 3, true)]; // Targetting OpenGL 4.3
            _display = new (new EglDisplayCreationOptions()
            {
                Egl = _interface,
                GlVersions = openglVersion
            });

            _context = _display.CreateContext(null);
#if DEBUG
            int[] attributes = [EglConsts.EGL_WIDTH, 1, EglConsts.EGL_HEIGHT, 1, EglConsts.EGL_NONE];
            nint pBuffer = _interface.CreatePBufferSurface(_display.Handle, _display.Config, attributes);
            if (pBuffer == nint.Zero) throw new Exception("EGL pbuffer not initialised");
            _surface = new (_display, pBuffer);
#else
            _surface = new (_display, _context.OffscreenSurface.DangerousGetHandle());
#endif

            this.SetContext(_context.Context);
        }

        private static string GetLibraryPath()
        {
            string libraryName = OperatingSystem.IsLinux() && !OperatingSystem.IsWindows() ? _eglLinux : _eglWin;
            return $"{Path.GetDirectoryName(Environment.ProcessPath)}/{libraryName}";
        }

        ~EglManager() => Dispose(false);

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
                _display.Dispose();
            }

            _disposed = true;
        }

        public OpenGLPlatformInfo GetOpenGLInfo() => new (
            _context.Context,
            this.GetProcAddress,
            this.SetContext,
            _interface.GetCurrentContext,
            () => this.SetContext(nint.Zero),
            (context) => _interface.DestroyContext(_display.Handle, context),
            this.SwapBuffers,
            this.SwapInterval
        );

        public string GetNativeInfo()
        {
            // Constants found in https://github.com/AvaloniaUI/Avalonia/blob/main/src/Avalonia.OpenGL/Egl/EglConsts.cs
            const int vendorId = 0x3053;
            const int versionId = 0x3054;

            string? vendorString = _interface.QueryString(_display.Handle, vendorId);
            string? versionString = _interface.QueryString(_display.Handle, versionId);

            return $"EGL {versionString} {vendorString}";
        }

        private nint GetProcAddress(string methodName) // Adapted from https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.OpenGL/GlInterface.cs
        {
            nint rawMethodName = Marshal.StringToHGlobalAnsi(methodName);
            nint funcAddress = _interface.GetProcAddress(rawMethodName);
            Marshal.FreeHGlobal(rawMethodName);
            return funcAddress;
        }

        private void SetContext(nint context)
        {
            nint currentDisplay = _display.Handle;
            nint currentSurface = _surface?.DangerousGetHandle() ?? nint.Zero;
            if (context == nint.Zero)
            {
                _interface.MakeCurrent(currentDisplay, nint.Zero, nint.Zero, context);
            } else
            {
                _interface.MakeCurrent(currentDisplay, currentSurface, currentSurface, context);
            }
        }

        private void SwapInterval(bool shouldSync)
        {
            nint currentDisplay = _display.Handle;
            _interface.SwapInterval(currentDisplay, shouldSync ? 1 : 0);
        }

        private void SwapBuffers()
        {
            if (_surface is not null)
            {
                _surface.SwapBuffers();
            } else
            {
                _interface.SwapBuffers(_display.Handle, 0);
            }
        }
    }
}