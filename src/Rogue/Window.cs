using Avalonia;
using Avalonia.Controls;

using Rogue.Controls;
using Rogue.Graphics;
using Rogue.Graphics.Backends;
using Rogue.Manager;

using Veldrid;

using WindowControl = Avalonia.Controls.Window;
using AvaloniaBrushes = Avalonia.Media.Brushes;
using Avalonia.Interactivity;

namespace Rogue
{
    internal class Window
    {
        private struct WindowDimensions
        {
            public uint Width;
            public uint Height;
        }

        private string _title = "New Window - Rogue";

        private WindowDimensions _dimensions;

        private TabManager _tabs;

        public Window(uint width, uint height, string url = "")
        {
            _dimensions = new ()
            {
                Width = width,
                Height = height
            };

            // this.InitialiseOpenGL();

            _tabs = new ();
            _tabs.CreateTab(url, true);
        }

        public void Init(Application app, string[] args)
        {
            Panel windowPanel = new ()
            {
                Background = AvaloniaBrushes.White
            };

            WebPage currentPage = (WebPage) _tabs.Current;
            Action renderWebPage = currentPage.RenderPage;

            // OpenGlCanvas canvas = new (_egl.GetOpenGLInfo(), renderWebPage);
            RogueCanvas canvas = new (renderWebPage, GraphicsResources.Device);

            windowPanel.Children.Add(canvas);

            WindowControl layout = new ()
            {
                Title = _title,
                Width = _dimensions.Width,
                Height = _dimensions.Height,
                Position = PixelPoint.Origin,
                Content = windowPanel
            };

            layout.Resized += this.OnResize;
            layout.Loaded += this.LoadGraphics;

            layout.Show();
            app.Run(layout);
        }

        private void LoadGraphics(object? sender, RoutedEventArgs args)
        {
            if (sender is not WindowControl window) return;

            GraphicsDeviceOptions opts = new ()
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true,
                Debug = true
            }; // Defaults taken from veldrid.dev tutorial
            
            if (TopLevel.GetTopLevel(window)?.TryGetPlatformHandle() is {} handle)
            {
                if (OperatingSystem.IsWindows())
                {
                    GraphicsResources.Device ??= GraphicsDevice.CreateD3D11(
                        opts,
                        handle.Handle,
                        _dimensions.Width,
                        _dimensions.Height
                    );
                } else if (OperatingSystem.IsLinux())
                {
                    if (handle.HandleDescriptor == "XID")
                    {
                        SwapchainDescription description = new (
                            SwapchainSource.CreateXlib(
                                X11Interop.XOpenDisplay(nint.Zero),
                                handle.Handle
                            ),
                            _dimensions.Width,
                            _dimensions.Height,
                            PixelFormat.D24_UNorm_S8_UInt,
                            true
                        );

                        GraphicsResources.Device ??= GraphicsDevice.CreateVulkan(
                            opts,
                            description
                        );
                    } // TODO: Add proper Wayland support
                } else if (OperatingSystem.IsMacOS())
                {
                    GraphicsResources.Device ??= GraphicsDevice.CreateMetal(
                        opts,
                        handle.Handle
                    );
                } else
                {
                    throw new Exception("Unsupported platform!");
                }
            }

            
            WebPage currentPage = (WebPage) _tabs.Current;
            currentPage.InitialiseGraphics();
            currentPage.RenderPage();
        }

        private void OnResize(object? sender, WindowResizedEventArgs args)
        {
            _dimensions.Width = Convert.ToUInt32(args.ClientSize.Width);
            _dimensions.Height = Convert.ToUInt32(args.ClientSize.Height);

            DrawingContext.SetOrthogonalMatrix(_dimensions.Width, _dimensions.Height);

            GraphicsResources.Device?.ResizeMainWindow(_dimensions.Width, _dimensions.Height);
        }
    }

    static partial class X11Interop
    {
        [System.Runtime.InteropServices.LibraryImport("libX11.so.6")]
        public static partial nint XOpenDisplay(nint display);
    }
}