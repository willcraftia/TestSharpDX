#region Using

using System;

using D3D11Texture2D = SharpDX.Direct3D11.Texture2D;
using DXGIFactory1 = SharpDX.DXGI.Factory1;
using DXGIFormat = SharpDX.DXGI.Format;
using DXGIModeDescription = SharpDX.DXGI.ModeDescription;
using DXGIPresentFlags = SharpDX.DXGI.PresentFlags;
using DXGISwapChain = SharpDX.DXGI.SwapChain;
using DXGISwapChainDescription = SharpDX.DXGI.SwapChainDescription;
using DXGISwapChainFlags = SharpDX.DXGI.SwapChainFlags;
using DXGISwapEffect = SharpDX.DXGI.SwapEffect;
using DXGIUsage = SharpDX.DXGI.Usage;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxSwapChain : ISwapChain
    {
        /// <summary>
        /// バック バッファ数。
        /// </summary>
        public const int BackBufferCount = 2;

        SdxDevice device;

        RefreshRate refreshRate;

        DXGISwapChainFlags dxgiSwapChainFlags;

        SdxRenderTarget renderTarget;

        public int BackBufferWidth { get; private set; }

        public int BackBufferHeight { get; private set; }

        public SurfaceFormat BackBufferFormat { get; private set; }

        public int BackBufferMultiSampleCount { get; private set; }

        public int BackBufferMultiSampleQuality { get; private set; }

        public bool Windowed { get; private set; }

        public bool AllowModeSwitch { get; private set; }

        public DepthFormat DepthStencilFormat { get; private set; }

        public int SyncInterval { get; private set; }

        public IRenderTargetView RenderTargetView { get; private set; }

        public DXGISwapChain DXGISwapChain { get; private set; }

        public SdxSwapChain(SdxDevice device, SwapChainSettings settings)
        {
            if (device == null) throw new ArgumentNullException("device");

            this.device = device;

            DepthStencilFormat = settings.DepthStencilFormat;
            SyncInterval = settings.SyncInterval;

            InitializeDXGISwapChain(ref settings);
            InitializeBackBuffer();
        }

        public void Present()
        {
            // TODO
            // DXGIPresentFlags を理解できていない。
            DXGISwapChain.Present(SyncInterval, DXGIPresentFlags.None);
        }

        public void ResizeBuffers(int width, int height)
        {
            ResizeBuffers(width, height, 1, BackBufferFormat);
        }

        public void ResizeBuffers(int width, int height, int bufferCount, SurfaceFormat format)
        {
            if (width < 0) throw new ArgumentOutOfRangeException("width");
            if (height < 0) throw new ArgumentOutOfRangeException("height");
            if (bufferCount < 0) throw new ArgumentOutOfRangeException("bufferCount");

            // ResizeBuffers
            //
            // バック バッファのサイズ、フォーマット、バッファ数を変更。
            // ウィンドウのサイズが変更された時に呼び出す必要がある。
            //
            // width = 0 や height = 0 の場合、
            // 対象ウィンドウのクライアント領域のサイズが用いられる。
            // bufferCount = 0 の場合、既存のバッファ数が保持される。
            // 

            BackBufferWidth = width;
            BackBufferHeight = height;
            BackBufferFormat = format;

            // ResizeBuffers の前には、スワップ チェーンに関連付けられた
            // 全てのリソースを解放しなければならない。
            ReleaseBackBuffer();

            DXGISwapChain.ResizeBuffers(bufferCount, width, height, (DXGIFormat) format, dxgiSwapChainFlags);
        }

        public void ResizeTarget()
        {
            ResizeTarget(BackBufferWidth, BackBufferHeight);
        }

        public void ResizeTarget(int width, int height)
        {
            // ResizeTarget
            //
            // ・ウィンドウ モードの場合は対象ウィンドウのサイズを変更
            // ・全画面モードの場合は表示モードを変更
            //
            // アプリケーションは、このメソッド呼び出しで対象ウィンドウのサイズを変更できます、とのこと。

            var description = new DXGIModeDescription
            {
                Width = width,
                Height = height,
                // リフレッシュ レート設定を維持。
                RefreshRate =
                {
                    Numerator = refreshRate.Numerator,
                    Denominator = refreshRate.Denominator
                },
                // フォーマット設定を維持。
                Format = (DXGIFormat) BackBufferFormat
            };

            DXGISwapChain.ResizeTarget(ref description);

        }

        void InitializeDXGISwapChain(ref SwapChainSettings settings)
        {
            int multiSampleQuality = 0;
            if (1 < settings.BackBufferMultiSampleCount)
            {
                // 注意:
                // Quality = 最大レベル - 1
                multiSampleQuality = device.CheckMultiSampleQualityLevels(
                    settings.BackBufferFormat, settings.BackBufferMultiSampleCount) - 1;
            }

            var dxgiSwapChainFlags = DXGISwapChainFlags.None;
            // DXGISwapChainFlags.AllowModeSwitch のみ対応。
            if (AllowModeSwitch)
                dxgiSwapChainFlags |= DXGISwapChainFlags.AllowModeSwitch;

            var description = new DXGISwapChainDescription
            {
                ModeDescription =
                {
                    Width = settings.BackBufferWidth,
                    Height = settings.BackBufferHeight,
                    RefreshRate =
                    {
                        Numerator = settings.BackBufferRefreshRate.Numerator,
                        Denominator = settings.BackBufferRefreshRate.Denominator
                    },
                    Format = (DXGIFormat) settings.BackBufferFormat
                },
                SampleDescription =
                {
                    Count = settings.BackBufferMultiSampleCount,
                    Quality = multiSampleQuality
                },
                Usage = DXGIUsage.RenderTargetOutput | DXGIUsage.ShaderInput,
                BufferCount = BackBufferCount,
                OutputHandle = settings.OutputWindow,
                IsWindowed = settings.Windowed,
                SwapEffect = DXGISwapEffect.Discard,
                Flags = dxgiSwapChainFlags
            };

            var dxgiFactory = (device.Adapter as SdxAdapter).DXGIAdapter.GetParent<DXGIFactory1>();
            DXGISwapChain = new DXGISwapChain(dxgiFactory, device.D3D11Device, description);

            // スワップ チェーンの生成では、推測により値が決定されるものも含まれるため、
            // スワップ チェーンの設定を示すプロパティについては、
            // スワップ チェーンの生成後に保持する。

            description = DXGISwapChain.Description;

            BackBufferWidth = description.ModeDescription.Width;
            BackBufferHeight = description.ModeDescription.Height;
            BackBufferFormat = (SurfaceFormat) description.ModeDescription.Format;
            refreshRate = new RefreshRate
            {
                Numerator = description.ModeDescription.RefreshRate.Numerator,
                Denominator = description.ModeDescription.RefreshRate.Denominator
            };
            BackBufferMultiSampleCount = description.SampleDescription.Count;
            BackBufferMultiSampleQuality = description.SampleDescription.Quality;
            Windowed = description.IsWindowed;

            dxgiSwapChainFlags = description.Flags;
        }

        void InitializeBackBuffer()
        {
            var depthStencil = device.CreateDepthStencil(
                BackBufferWidth,
                BackBufferHeight,
                DepthStencilFormat,
                BackBufferMultiSampleCount,
                BackBufferMultiSampleQuality);

            var backBuffer = DXGISwapChain.GetBackBuffer<D3D11Texture2D>(0);

            // バッファ リサイズ時にバッファの破棄が発生するため、
            // 深度ステンシルを共有している設定は自由に破棄できずに都合が悪い。
            // よって、共有不可 (RenderTargetUsage.Preserve) でレンダ ターゲットを生成。
            renderTarget = new SdxRenderTarget(backBuffer, RenderTargetUsage.Preserve, depthStencil);
            renderTarget.Name = "BackBuffer_0";

            RenderTargetView = device.CreateRenderTargetView(renderTarget);
        }

        void ReleaseBackBuffer()
        {
            if (renderTarget != null)
            {
                renderTarget.Dispose();
                renderTarget = null;
            }

            if (RenderTargetView != null)
            {
                RenderTargetView.Dispose();
                RenderTargetView = null;
            }
        }

        #region IDisposable

        bool disposed;

        ~SdxSwapChain()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                RenderTargetView.Dispose();
                DXGISwapChain.Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
