#region Using

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Libra.Graphics;

#endregion

namespace Libra.Games
{
    public class GraphicsManager : IGraphicsManager, IGraphicsService, IDisposable
    {
        // メモ
        //
        // DeviceLost:
        //      D3D11 ではデバイス ロストが無いので不要。
        //
        // DeviceResetting/DeviceReset:
        //      デバイスのリセットも考える必要がないのでは？
        //      あり得るのはアダプタあるいはプロファイルの変更だが。
        //      自分には重要ではないので DeviceResetting/DeviceReset イベントは不要。
        //      アダプタやプロファイルを変更したいなら、アプリケーション全体を再初期化すれば良い。
        //
        // PreparingDeviceSettings:
        //      GrahicsDeviceInformation から SwapChain 設定を除外し、アダプタと表示モードに特化させたため、
        //      生成されたインスタンスをイベントで受信して修正することはもうできない。
        //      2D 用の深度ステンシル バッファ無効化ならば、PreferredDepthStencilFormat で None を指定すれば良い。
        //      よって、PreparingDeviceSettings イベントは不要。
        //
        // FindBestDevice/RankDevices
        //      アダプタ選択とモニタの表示モード選択を分離。
        //      および、それらをオーバライド可能なメソッドで定義。
        //

        public event EventHandler DeviceCreated;

        public event EventHandler DeviceDisposing;
        
        public event EventHandler Disposed;

        // D3D11.h: D3D11_MAX_MULTISAMPLE_SAMPLE_COUNT ( 32 )
        // TODO
        // 別の所で定義すべきでは？
        public const int MaxMultiSampleCount = 32;

        public static readonly int DefaultBackBufferWidth = 800;
        
        public static readonly int DefaultBackBufferHeight = 600;

        static readonly DeviceProfile[] profiles =
        {
            DeviceProfile.Level_11_0,
            DeviceProfile.Level_10_1,
            DeviceProfile.Level_10_0,
        };

        Game game;

        IGraphicsFactory graphicsFactory;

        int preferredBackBufferWidth;

        int preferredBackBufferHeight;

        SurfaceFormat preferredBackBufferFormat;

        DepthFormat preferredDepthStencilFormat;

        object resizeSwapChainLock = new object();

        public DeviceProfile Profile { get; set; }

        public IDevice Device { get; private set; }

        public ISwapChain SwapChain { get; private set; }

        public int PreferredBackBufferWidth
        {
            get { return preferredBackBufferWidth; }
            set
            {
                if (preferredBackBufferWidth <= 0) throw new ArgumentOutOfRangeException("value");
                preferredBackBufferWidth = value;
            }
        }

        public int PreferredBackBufferHeight
        {
            get { return preferredBackBufferHeight; }
            set
            {
                if (preferredBackBufferHeight <= 0) throw new ArgumentOutOfRangeException("value");
                preferredBackBufferHeight = value;
            }
        }

        public SurfaceFormat PreferredBackBufferFormat
        {
            get { return preferredBackBufferFormat; }
            set
            {
                // 32 ビットを越えるフォーマットを拒否。
                if (value != SurfaceFormat.Bgr565 &&
                    value != SurfaceFormat.Bgra5551 &&
                    value != SurfaceFormat.Color &&
                    value != SurfaceFormat.Rgba1010102)
                    throw new ArgumentOutOfRangeException("value");

                preferredBackBufferFormat = value;
            }
        }

        public DepthFormat PreferredDepthStencilFormat
        {
            get { return preferredDepthStencilFormat; }
            set { preferredDepthStencilFormat = value; }
        }

        public bool PreferMultiSampling { get; set; }

        public bool SynchronizeWithVerticalRetrace { get; set; }

        public bool Windowed { get; set; }

        public GraphicsManager(Game game)
        {
            if (game == null) throw new ArgumentNullException("game");
            if (game.Services.GetService(typeof(IGraphicsService)) != null)
                throw new ArgumentException("GraphicsDeviceManager is already registered.", "game");

            this.game = game;

            Profile = DeviceProfile.Level_11_0;
            preferredBackBufferWidth = DefaultBackBufferWidth;
            preferredBackBufferHeight = DefaultBackBufferHeight;
            preferredBackBufferFormat = SurfaceFormat.Color;
            preferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            Windowed = true;

            game.Services.AddService<IGraphicsManager>(this);
            game.Services.AddService<IGraphicsService>(this);
        }

        public void Initialize()
        {
            // ファクトリの取得。
            var gamePlatform = game.Services.GetRequiredService<IGamePlatform>();
            graphicsFactory = gamePlatform.GraphicsFactory;

            InitializeDevice();
            InitializeSwapChain();

            // ウィンドウあるいは全画面をリサイズ。
            // デバイス再構築の場合は、それ自体が通常の状態ではないので、強制リサイズとする。
            SwapChain.ResizeTarget();
        }

        void InitializeDevice()
        {
            var settings = new DeviceSettings();

            var adapter = ResolveAdapter();

            Device = graphicsFactory.CreateDevice(adapter, settings, profiles);
            Device.Disposing += OnDeviceDisposing;

            OnDeviceCreated(this, EventArgs.Empty);
        }

        void InitializeSwapChain()
        {
            DisplayMode backBufferMode;
            if (Windowed)
            {
                var monitorMode = ResolveOutputMode(Device);

                backBufferMode = new DisplayMode
                {
                    Width = preferredBackBufferWidth,
                    Height = preferredBackBufferHeight,
                    Format = preferredBackBufferFormat,
                    // 現在の設定と思われるモニタのリフレッシュ レートに従う。
                    RefreshRate = monitorMode.RefreshRate
                };
            }
            else
            {
                backBufferMode = ResolveFullScreenBackBufferMode(Device);
            }

            var settings = new SwapChainSettings
            {
                BackBufferWidth = backBufferMode.Width,
                BackBufferHeight = backBufferMode.Height,
                BackBufferRefreshRate = backBufferMode.RefreshRate,
                BackBufferFormat = backBufferMode.Format,
                BackBufferMultiSampleCount = 2,
                DepthStencilFormat = preferredDepthStencilFormat,
                // TODO
                // false にしても [alt]+[Enter] が有効なのだが？
                AllowModeSwitch = true,
                SyncInterval = (SynchronizeWithVerticalRetrace) ? 1 : 0,
                Windowed = Windowed,
                OutputWindow = game.Window.Handle
            };

            // スワップ チェーンの初期化。
            SwapChain = graphicsFactory.CreateSwapChain(Device, settings);
        }

        protected virtual IAdapter ResolveAdapter()
        {
            // デフォルト実装ではデフォルト アダプタを利用。
            // 推奨設定に見合う表示モードをデフォルト アダプタが提供できるか否かを考慮しない。

            return graphicsFactory.DefaultAdapter;
        }

        protected virtual DisplayMode ResolveOutputMode(IDevice device)
        {
            // デフォルト実装では、デフォルト アダプタの主モニタを対象に、
            // デスクトップへの出力に相応しいモードを検索。

            var output = graphicsFactory.DefaultAdapter.PrimaryOutput;

            // 主モニタよりデスクトップ サイズを取得。
            var desktopBounds = output.DesktopCoordinates;

            // デスクトップと同サイズ、かつ、SurfaceFormat.Color が条件。
            var preferredMode = new DisplayMode
            {
                Width = desktopBounds.Width,
                Height = desktopBounds.Height,
                Format = SurfaceFormat.Color
            };

            // デスクトップ出力に用いられている表示モードを検索。
            DisplayMode resolvedMode;
            output.GetClosestMatchingMode(device, ref preferredMode, out resolvedMode);

            return resolvedMode;
        }

        protected virtual DisplayMode ResolveFullScreenBackBufferMode(IDevice device)
        {
            // デフォルト実装では、推奨設定に最も適合する表示モードを、
            // DXGI の機能により主モニタの表示モードから自動選択。

            // リフレッシュ レート未指定で探索。
            var preferredMode = new DisplayMode
            {
                Width = preferredBackBufferWidth,
                Height = preferredBackBufferHeight,
                Format = preferredBackBufferFormat
            };

            DisplayMode resolvedMode;
            device.Adapter.PrimaryOutput.GetClosestMatchingMode(device, ref preferredMode, out resolvedMode);

            return resolvedMode;
        }

        public bool BeginDraw()
        {
            if (Device == null)
                return false;

            var context = Device.ImmediateContext;

            context.OutputMergerStage.SetRenderTargetView(SwapChain.RenderTargetView);

            return true;
        }

        public void EndDraw()
        {
            SwapChain.Present();
        }

        public void ToggleFullScreen()
        {

        }

        protected virtual void OnDeviceCreated(Object sender, EventArgs e)
        {
            if (DeviceCreated != null)
                DeviceCreated(this, e);
        }

        protected virtual void OnDeviceDisposing(object sender, EventArgs e)
        {
            if (DeviceDisposing != null)
                DeviceDisposing(this, e);
        }

        #region IDisposable

        bool disposed;

        ~GraphicsManager()
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
                if (SwapChain != null)
                    SwapChain.Dispose();

                if (Device != null)
                    Device.Dispose();
            }

            disposed = true;

            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }

        #endregion
    }
}
