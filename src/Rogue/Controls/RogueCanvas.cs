
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;

using SkiaSharp;

using Veldrid;

namespace Rogue.Controls
{
    public class RogueCanvas(
        Action renderProc,
        GraphicsDevice? graphicsDevice
    ): Control, ICustomDrawOperation
    {
        private readonly Action _renderProc = renderProc;

        private readonly GraphicsDevice? _gd = graphicsDevice;

        public override void Render(DrawingContext context)
        {
            if (_gd is null) return;
            context.Custom(this);
            Dispatcher.UIThread.InvokeAsync(this.InvalidateVisual);
            base.Render(context);
        }

        public void Render(ImmediateDrawingContext context)
        {
            if (_gd is not null)
            {
                _renderProc.Invoke();


                var feature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
                if (feature is null) return;

                using ISkiaSharpApiLease lease = feature.Lease();
                SKCanvas canvas = lease.SkCanvas;

                using SKSurface? fbSurface = _gd?.BackendType switch
                {
                    GraphicsBackend.Vulkan => this.GetVulkanRenderTarget(),
                    _ => throw new NotImplementedException("This backend has not been implemented")
                };

                canvas.DrawSurface(fbSurface, 0, 0);

            }


            // using GRContext backendContext = GRContext.
        }

        public SKSurface GetVulkanRenderTarget()
        {
            BackendInfoVulkan vulkanInfo = _gd!.GetVulkanInfo();

            nint GetProcAddress(string name, nint instanceHandle, nint deviceHandle)
            {
                nint zero = nint.Zero;
                if (instanceHandle == zero)
                {
                    return vulkanInfo.DeviceProcAddr(name);
                } else if (deviceHandle == zero)
                {
                    return vulkanInfo.InstanceProcAddr(name);
                } else
                {
                    return zero;
                }
            }

            // Why?!
            GraphicsApiVersion apiVersion = _gd.ApiVersion;
            uint vulkanVersion = 0;
            
            vulkanVersion |= (uint) apiVersion.Major << 22;
            vulkanVersion |= (uint) apiVersion.Minor << 12;
            vulkanVersion |= (uint) apiVersion.Patch;

            using GRVkExtensions vkExtensions = GRVkExtensions.Create(
                GetProcAddress,
                vulkanInfo.Instance,
                vulkanInfo.PhysicalDevice,
                vulkanInfo.AvailableInstanceExtensions.ToArray(),
                vulkanInfo.AvailableDeviceExtensions.Select(x => x.ToString()).ToArray()
            );

            using GRContext backendContext = GRContext.CreateVulkan(
                new GRVkBackendContext()
                {
                    Extensions = vkExtensions,
                    GetProcedureAddress = GetProcAddress,
                    GraphicsQueueIndex = vulkanInfo.GraphicsQueueFamilyIndex,
                    MaxAPIVersion = vulkanVersion,
                    ProtectedContext = false,
                    VkDevice = vulkanInfo.Device,
                    VkInstance = vulkanInfo.Instance,
                    VkPhysicalDevice = vulkanInfo.PhysicalDevice,
                    VkPhysicalDeviceFeatures = vulkanInfo.PhysicalDeviceFeatures,
                    VkPhysicalDeviceFeatures2 = nint.Zero,
                    VkQueue = vulkanInfo.GraphicsQueue
                }
            );

            Texture fboTexture = _gd.SwapchainFramebuffer!.ColorTargets[0].Target;
            var conversionInfo = new GrVkYcbcrConversionInfo()
            {
                Format = (uint) vulkanInfo.GetVkFormat(fboTexture),
                ExternalFormat = 0,
                YcbcrModel = 0,
                YcbcrRange = 0,
                ChromaFilter = 1,
                XChromaOffset = 0,
                YChromaOffset = 0,
                ForceExplicitReconstruction = 0
            };

            var memInfo = vulkanInfo.GetImageMemoryInfo(fboTexture);

            GRVkAlloc vkAllocInfo = new ()
            {
                BackendMemory = memInfo.MemoryBuffer,
                Flags = 0,
                Memory = memInfo.MemoryHandle,
                Offset = memInfo.Offset,
                Size = memInfo.Size
            };

            GRVkImageInfo fboInfo = new ()
            {
                Alloc = vkAllocInfo,
                CurrentQueueFamily = vulkanInfo.GraphicsQueueFamilyIndex,
                Format = (uint) vulkanInfo.GetVkFormat(fboTexture),
                Image = vulkanInfo.GetVkImage(fboTexture),
                ImageLayout = (uint) vulkanInfo.GetImageLayout(fboTexture),
                ImageTiling = (fboTexture.Usage & TextureUsage.Staging) == TextureUsage.Staging ? (uint) 1 : 0,
                LevelCount = fboTexture.MipLevels,
                Protected = false,
                SampleCount = RogueCanvas.GetSampleCount(fboTexture.SampleCount),
                SharingMode = 0,
                YcbcrConversionInfo = conversionInfo
            };

            using GRBackendRenderTarget vulkanRenderTarget = new (
                (int) fboTexture.Width,
                (int) fboTexture.Height,
                fboInfo
            );

            return SKSurface.Create(backendContext, vulkanRenderTarget, SKColorType.Rgba8888);
        }

        private static uint GetSampleCount(TextureSampleCount sampleCount) => sampleCount switch
        {
            TextureSampleCount.Count1 => 1,
            TextureSampleCount.Count2 => 2,
            TextureSampleCount.Count4 => 4,
            TextureSampleCount.Count8 => 8,
            TextureSampleCount.Count16 => 16,
            TextureSampleCount.Count32 => 32,
            TextureSampleCount.Count64 => 64,
            _ => 0
        };

        public bool HitTest(Point p) => this.Bounds.Contains(p);

        public bool Equals(ICustomDrawOperation? op) => false;

        public void Dispose() { }
    }
}