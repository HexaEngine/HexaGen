namespace D3D11Test
{
    using Hexa.NET.D3D11;
    using Hexa.NET.DXGI;
    using HexaGen.Runtime;
    using HexaGen.Runtime.COM;
    using System.Runtime.InteropServices;
    using System.Text;

    public unsafe class DXGIAdapter : IDisposable
    {
        private bool disposedValue;
        private ComPtr<IDXGIFactory7> factory;
        private ComPtr<IDXGIAdapter4> adapter;

        private ComPtr<IDXGIDebug> dxgiDebug;
        private ComPtr<IDXGIInfoQueue> infoQueue;

        private readonly Guid DXGI_DEBUG_ALL = new(0xe48ae283, 0xda80, 0x490b, 0x87, 0xe6, 0x43, 0xe9, 0xa9, 0xcf, 0xda, 0x8);
        private readonly Guid DXGI_DEBUG_DX = new(0x35cdd7fc, 0x13b2, 0x421d, 0xa5, 0xd7, 0x7e, 0x44, 0x51, 0x28, 0x7d, 0x64);
        private readonly Guid DXGI_DEBUG_DXGI = new(0x25cddaa4, 0xb1c6, 0x47e1, 0xac, 0x3e, 0x98, 0x87, 0x5b, 0x5a, 0x2e, 0x2a);
        private readonly Guid DXGI_DEBUG_APP = new(0x6cd6e01, 0x4219, 0x4ebd, 0x87, 0x9, 0x27, 0xed, 0x23, 0x36, 0xc, 0x62);
        private readonly Guid DXGI_DEBUG_D3D11 = new(0x4b99317b, 0xac39, 0x4aa6, 0xbb, 0xb, 0xba, 0xa0, 0x47, 0x84, 0x79, 0x8f);
        private readonly bool debug;

        public DXGIAdapter(bool debug)
        {
            Console.WriteLine("DXGI Init");

            if (debug)
            {
                DXGI.DXGIGetDebugInterface1(0, out dxgiDebug);
                DXGI.DXGIGetDebugInterface1(0, out infoQueue);

                DxgiInfoQueueFilter filter = new();
                filter.DenyList.NumIDs = 1;
                filter.DenyList.PIDList = (int*)AllocT(D3D11MessageId.SetprivatedataChangingparams);
                infoQueue.AddStorageFilterEntries(DXGI_DEBUG_ALL, &filter);
                infoQueue.SetBreakOnSeverity(DXGI_DEBUG_ALL, DxgiInfoQueueMessageSeverity.Message, false);
                infoQueue.SetBreakOnSeverity(DXGI_DEBUG_ALL, DxgiInfoQueueMessageSeverity.Info, false);
                infoQueue.SetBreakOnSeverity(DXGI_DEBUG_ALL, DxgiInfoQueueMessageSeverity.Warning, true);
                infoQueue.SetBreakOnSeverity(DXGI_DEBUG_ALL, DxgiInfoQueueMessageSeverity.Error, true);
                infoQueue.SetBreakOnSeverity(DXGI_DEBUG_ALL, DxgiInfoQueueMessageSeverity.Corruption, true);
                Free(filter.DenyList.PIDList);
            }
            DXGI.CreateDXGIFactory2(debug ? DXGI.DXGI_CREATE_FACTORY_DEBUG : 0u, out factory);

            adapter = GetHardwareAdapter();
            this.debug = debug;

            Console.WriteLine("DXGI Init Done");
        }

        public ComPtr<IDXGIFactory7> Factory => factory;

        public ComPtr<IDXGIAdapter4> Adapter => adapter;

        public static string Convert(DxgiInfoQueueMessageSeverity severity)
        {
            return severity switch
            {
                DxgiInfoQueueMessageSeverity.Corruption => "CORRUPTION",
                DxgiInfoQueueMessageSeverity.Error => "ERROR",
                DxgiInfoQueueMessageSeverity.Warning => "WARNING",
                DxgiInfoQueueMessageSeverity.Info => "INFO",
                DxgiInfoQueueMessageSeverity.Message => "LOG",
                _ => throw new NotImplementedException(),
            };
        }

        public static string Convert(DxgiInfoQueueMessageCategory category)
        {
            return category switch
            {
                DxgiInfoQueueMessageCategory.Unknown => "DXGI_INFO_QUEUE_MESSAGE_CATEGORY_UNKNOWN",
                DxgiInfoQueueMessageCategory.Miscellaneous => "DXGI_INFO_QUEUE_MESSAGE_CATEGORY_MISCELLANEOUS",
                DxgiInfoQueueMessageCategory.Initialization => "DXGI_INFO_QUEUE_MESSAGE_CATEGORY_INITIALIZATION",
                DxgiInfoQueueMessageCategory.Cleanup => "DXGI_INFO_QUEUE_MESSAGE_CATEGORY_CLEANUP",
                DxgiInfoQueueMessageCategory.Compilation => "DXGI_INFO_QUEUE_MESSAGE_CATEGORY_COMPILATION",
                DxgiInfoQueueMessageCategory.StateCreation => "DXGI_INFO_QUEUE_MESSAGE_CATEGORY_STATE_CREATION",
                DxgiInfoQueueMessageCategory.StateSetting => "DXGI_INFO_QUEUE_MESSAGE_CATEGORY_STATE_SETTING",
                DxgiInfoQueueMessageCategory.StateGetting => "DXGI_INFO_QUEUE_MESSAGE_CATEGORY_STATE_GETTING",
                DxgiInfoQueueMessageCategory.ResourceManipulation => "DXGI_INFO_QUEUE_MESSAGE_CATEGORY_RESOURCE_MANIPULATION",
                DxgiInfoQueueMessageCategory.Execution => "DXGI_INFO_QUEUE_MESSAGE_CATEGORY_EXECUTION",
                DxgiInfoQueueMessageCategory.Shader => "DXGI_INFO_QUEUE_MESSAGE_CATEGORY_SHADER",
                _ => throw new NotImplementedException(),
            };
        }

        public void PumpDebugMessages()
        {
            if (!debug)
                return;
            ulong messageCount = infoQueue.GetNumStoredMessages(DXGI_DEBUG_ALL);
            for (ulong i = 0; i < messageCount; i++)
            {
                nuint messageLength;

                HResult hr = infoQueue.GetMessageA(DXGI_DEBUG_ALL, i, (DxgiInfoQueueMessage*)null, &messageLength);

                if (hr.IsSuccess)
                {
                    DxgiInfoQueueMessage* message = (DxgiInfoQueueMessage*)Alloc(messageLength);

                    hr = infoQueue.GetMessageA(DXGI_DEBUG_ALL, i, message, &messageLength);

                    if (hr.IsSuccess)
                    {
                        string msg = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(message->PDescription));

                        if (message->Producer == DXGI_DEBUG_DX)
                            Console.WriteLine($"DX {Convert(message->Severity)}: {msg} [ {Convert(message->Category)} ]");
                        if (message->Producer == DXGI_DEBUG_DXGI)
                            Console.WriteLine($"DXGI {Convert(message->Severity)}: {msg} [ {Convert(message->Category)} ]");
                        if (message->Producer == DXGI_DEBUG_APP)
                            Console.WriteLine($"APP {Convert(message->Severity)}: {msg} [ {Convert(message->Category)} ]");
                        if (message->Producer == DXGI_DEBUG_D3D11)
                            Console.WriteLine($"D3D11 {Convert(message->Severity)}: {msg} [ {Convert(message->Category)} ]");

                        Free(message);
                    }
                }
            }

            infoQueue.ClearStoredMessages(DXGI_DEBUG_ALL);
        }

        public DXGISwapChain CreateSwapChain(D3D11DeviceManager manager, SDLWindow window)
        {
            var (Hwnd, HDC, HInstance) = window.Win32 ?? throw new NotSupportedException();
            DxgiSwapChainDesc1 desc = new()
            {
                Width = (uint)window.Width,
                Height = (uint)window.Height,
                Format = DxgiFormat.Formatb8G8R8A8Unorm,
                BufferCount = 3,
                BufferUsage = (uint)DXGI.DXGI_USAGE_RENDER_TARGET_OUTPUT,
                SampleDesc = new(1, 0),
                Scaling = DxgiScaling.Stretch,
                SwapEffect = DxgiSwapEffect.FlipSequential,
                Flags = (uint)(DxgiSwapChainFlag.AllowModeSwitch | DxgiSwapChainFlag.AllowTearing),
                Stereo = false,
            };

            DxgiSwapChainFullscreenDesc fullscreenDesc = new()
            {
                Windowed = 1,
                RefreshRate = new DxgiRational(0, 1),
                Scaling = DxgiModeScaling.Unspecified,
                ScanlineOrdering = DxgiModeScanlineOrder.Unspecified,
            };

            factory.CreateSwapChainForHwnd((IUnknown*)manager.Device.Handle, Hwnd, &desc, &fullscreenDesc, (IDXGIOutput*)null, out ComPtr<IDXGISwapChain1> swapChain);

            return new DXGISwapChain(manager, swapChain, desc);
        }

        private ComPtr<IDXGIAdapter4> GetHardwareAdapter()
        {
            ComPtr<IDXGIAdapter4> selected = null;
            for (uint adapterIndex = 0;
                factory.EnumAdapterByGpuPreference(adapterIndex, DxgiGpuPreference.HighPerformance, out ComPtr<IDXGIAdapter4> adapter) !=
                (int)ResultCode.DXGI_ERROR_NOT_FOUND;
                adapterIndex++)
            {
                DxgiAdapterDesc1 desc;
                adapter.GetDesc1(&desc);

                if (((DxgiAdapterFlag)desc.Flags & DxgiAdapterFlag.Software) != DxgiAdapterFlag.None)
                {
                    // Don't select the Basic Render Driver adapter.
                    adapter.Release();
                    continue;
                }

                selected = adapter;
            }

            if (selected.Handle == null)
                throw new NotSupportedException();
            return selected;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                factory.Dispose();
                adapter.Dispose();
                disposedValue = true;
            }
        }

        ~DXGIAdapter()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}