#region Using

using System;
using System.Collections.Generic;

using D3D11Blend = SharpDX.Direct3D11.BlendOption;
using D3D11BlendOperation = SharpDX.Direct3D11.BlendOperation;
using D3D11BlendState = SharpDX.Direct3D11.BlendState;
using D3D11BlendStateDescription = SharpDX.Direct3D11.BlendStateDescription;
using D3D11ColorWriteMaskFlags = SharpDX.Direct3D11.ColorWriteMaskFlags;
using D3D11Comparison = SharpDX.Direct3D11.Comparison;
using D3D11CullMode = SharpDX.Direct3D11.CullMode;
using D3D11DepthStencilState = SharpDX.Direct3D11.DepthStencilState;
using D3D11DepthStencilStateDescription = SharpDX.Direct3D11.DepthStencilStateDescription;
using D3D11DepthWriteMask = SharpDX.Direct3D11.DepthWriteMask;
using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11DeviceChild = SharpDX.Direct3D11.DeviceChild;
using D3D11DeviceContext = SharpDX.Direct3D11.DeviceContext;
using D3D11Feature = SharpDX.Direct3D11.Feature;
using D3D11FillMode = SharpDX.Direct3D11.FillMode;
using D3D11Filter = SharpDX.Direct3D11.Filter;
using D3D11RasterizerState = SharpDX.Direct3D11.RasterizerState;
using D3D11RasterizerStateDescription = SharpDX.Direct3D11.RasterizerStateDescription;
using D3D11SamplerState = SharpDX.Direct3D11.SamplerState;
using D3D11SamplerStateDescription = SharpDX.Direct3D11.SamplerStateDescription;
using D3D11StencilOperation = SharpDX.Direct3D11.StencilOperation;
using D3D11TextureAddressMode = SharpDX.Direct3D11.TextureAddressMode;
using D3DDriverType = SharpDX.Direct3D.DriverType;
using D3DFeatureLevel = SharpDX.Direct3D.FeatureLevel;
using DXGIFormat = SharpDX.DXGI.Format;
using SDXSharpDXException = SharpDX.SharpDXException;

#endregion

namespace Libra.Graphics
{
    public sealed class Device : IDisposable
    {
        #region StateCollection

        public abstract class StateCollection<T> : IDisposable where T : State
        {
            internal Device Device { get; private set; }

            internal Dictionary<T, D3D11DeviceChild> CoresByState { get; private set; }

            internal StateCollection(Device device)
            {
                Device = device;

                CoresByState = new Dictionary<T, D3D11DeviceChild>();
            }

            /// <summary>
            /// ステートを追加します。
            /// </summary>
            /// <remarks>
            /// ステートを追加すると、対応する ID3D11DeviceChild が内部で生成されます。
            /// そして、生成された ID3D11DeviceChild は、ステートをキーとするディクショナリで管理されます。
            /// なお、追加されたステートはプロパティ凍結状態に設定され、
            /// 以後、プロパティを変更しようとすると例外が発生するようになります。
            /// </remarks>
            /// <param name="state">追加するステート。</param>
            public void Add(T state)
            {
                if (CoresByState.ContainsKey(state))
                    return;

                CoresByState[state] = CreateCore(state, Device);

                state.Freeze();
            }

            /// <summary>
            /// ステートを削除します。
            /// </summary>
            /// <remarks>
            /// ステートの削除では、対応する ID3D11DeviceChild の Dipose() が呼び出されます。
            /// また、削除したステートはプロパティ凍結状態が維持されます。
            /// </remarks>
            /// <param name="state">削除するステート。</param>
            public void Remove(T state)
            {
                if (!CoresByState.ContainsKey(state))
                    return;

                var core = CoresByState[state];
                if (!core.IsDisposed)
                    core.Dispose();

                CoresByState.Remove(state);
            }

            internal abstract D3D11DeviceChild CreateCore(T state, Device device);

            #region IDisposable

            bool disposed;

            ~StateCollection()
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
                    foreach (var core in CoresByState.Values)
                    {
                        // 子要素は同一ステートの場合に共有される可能性があるため、
                        // 念のため IsDisposed フラグで確認。
                        if (!core.IsDisposed)
                            core.Dispose();
                    }
                }

                disposed = true;
            }

            #endregion
        }

        #endregion

        #region BlendStateCollection

        public sealed class BlendStateCollection : StateCollection<BlendState>
        {
            internal D3D11BlendState this[BlendState state]
            {
                get { return CoresByState[state] as D3D11BlendState; }
            }

            internal BlendStateCollection(Device device)
                : base(device)
            {
            }

            internal override D3D11DeviceChild CreateCore(BlendState state, Device device)
            {
                var description = D3D11BlendStateDescription.Default();

                description.AlphaToCoverageEnable = false;
                description.IndependentBlendEnable = false;

                description.RenderTarget[0].IsBlendEnabled = true;
                description.RenderTarget[0].SourceBlend = (D3D11Blend) state.ColorSourceBlend;
                description.RenderTarget[0].DestinationBlend = (D3D11Blend) state.ColorDestinationBlend;
                description.RenderTarget[0].BlendOperation = (D3D11BlendOperation) state.ColorBlendFunction;
                description.RenderTarget[0].SourceAlphaBlend = (D3D11Blend) state.AlphaSourceBlend;
                description.RenderTarget[0].DestinationAlphaBlend = (D3D11Blend) state.AlphaDestinationBlend;
                description.RenderTarget[0].AlphaBlendOperation = (D3D11BlendOperation) state.AlphaBlendFunction;
                description.RenderTarget[0].RenderTargetWriteMask = (D3D11ColorWriteMaskFlags) state.ColorWriteChannels;

                // TODO
                // ColorWriteChannels1-3 は RenderTarget[1]-RenderTarget[3] か？

                return new D3D11BlendState(device.D3D11Device, description);
            }
        }

        #endregion

        #region SamplerStateCollection

        public sealed class SamplerStateCollection : StateCollection<SamplerState>
        {
            internal D3D11SamplerState this[SamplerState state]
            {
                get { return CoresByState[state] as D3D11SamplerState; }
            }

            internal SamplerStateCollection(Device device)
                : base(device)
            {
            }

            internal override D3D11DeviceChild CreateCore(SamplerState state, Device device)
            {
                var description = D3D11SamplerStateDescription.Default();

                description.AddressU = (D3D11TextureAddressMode) state.AddressU;
                description.AddressV = (D3D11TextureAddressMode) state.AddressV;
                description.AddressW = (D3D11TextureAddressMode) state.AddressW;
                description.Filter = (D3D11Filter) state.Filter;
                description.MaximumAnisotropy = state.MaxAnisotropy;
                description.MipLodBias = state.MipMapLodBias;
                description.ComparisonFunction = (D3D11Comparison) state.ComparisonFunction;
                description.BorderColor = state.BorderColor.ToSDXColor4();
                description.MinimumLod = state.MinLod;
                description.MaximumLod = state.MaxLod;

                return new D3D11SamplerState(device.D3D11Device, description);
            }
        }

        #endregion

        #region RasterizerStateCollection

        public sealed class RasterizerStateCollection : StateCollection<RasterizerState>
        {
            internal D3D11RasterizerState this[RasterizerState state]
            {
                get { return CoresByState[state] as D3D11RasterizerState; }
            }

            internal RasterizerStateCollection(Device device)
                : base(device)
            {
            }

            internal override D3D11DeviceChild CreateCore(RasterizerState state, Device device)
            {
                var description = new D3D11RasterizerStateDescription
                {
                    FillMode = (D3D11FillMode) state.FillMode,
                    CullMode = (D3D11CullMode) state.CullMode,
                    IsFrontCounterClockwise = false,
                    DepthBias = state.DepthBias,
                    SlopeScaledDepthBias = state.SlopeScaleDepthBias,
                    DepthBiasClamp = state.DepthBiasClamp,
                    IsDepthClipEnabled = state.DepthClipEnable,
                    IsScissorEnabled = state.ScissorEnable,
                    IsMultisampleEnabled = state.MultisampleEnable,
                    IsAntialiasedLineEnabled = state.AntialiasedLineEnable
                };

                return new D3D11RasterizerState(device.D3D11Device, description);
            }
        }

        #endregion

        #region DepthStencilStateCollection

        public sealed class DepthStencilStateCollection : StateCollection<DepthStencilState>
        {
            internal D3D11DepthStencilState this[DepthStencilState state]
            {
                get { return CoresByState[state] as D3D11DepthStencilState; }
            }

            internal DepthStencilStateCollection(Device device)
                : base(device)
            {
            }

            internal override D3D11DeviceChild CreateCore(DepthStencilState state, Device device)
            {
                var description = new D3D11DepthStencilStateDescription
                {
                    IsDepthEnabled = state.DepthEnable,
                    DepthWriteMask = (state.DepthWriteEnable) ? D3D11DepthWriteMask.All : D3D11DepthWriteMask.Zero,
                    DepthComparison = (D3D11Comparison) state.DepthFunction,
                    IsStencilEnabled = state.StencilEnable,
                    StencilReadMask = state.StencilReadMask,
                    StencilWriteMask = state.StencilWriteMask,
                    FrontFace =
                    {
                        FailOperation = (D3D11StencilOperation) state.FrontFaceStencilFail,
                        DepthFailOperation = (D3D11StencilOperation) state.FrontFaceStencilDepthFail,
                        PassOperation = (D3D11StencilOperation) state.FrontFaceStencilPass,
                        Comparison = (D3D11Comparison) state.FrontFaceStencilFunction
                    },
                    BackFace =
                    {
                        FailOperation = (D3D11StencilOperation) state.BackFaceStencilFail,
                        DepthFailOperation = (D3D11StencilOperation) state.BackFaceStencilDepthFail,
                        PassOperation = (D3D11StencilOperation) state.BackFaceStencilPass,
                        Comparison = (D3D11Comparison) state.BackFaceStencilFunction
                    }
                };

                return new D3D11DepthStencilState(device.D3D11Device, description);
            }
        }

        #endregion

        // D3D11 ではデバイス ロストが無いので DeviceLost イベントは不要。

        // デバイスのリセットも考える必要がないのでは？
        // あり得るとしたらアダプタあるいはプロファイルの変更だが・・・そんな事する？
        // 自分には重要ではないので DeviceResetting/DeviceReset イベントは不要。
        // アダプタやプロファイルを変更したいなら、アプリケーション全体を再初期化すれば良い。

        // ResourceCreated/ResourceDestroyed イベントは実装しない。
        // ShaprDX では、各リソースに対応するクラスのコンストラクタで
        // Device のリソース生成メソッドを隠蔽しているため、
        // イベントを発生させるためのトリガーを作れない。

        public event EventHandler Disposing;
        
        public readonly DeviceSettings Settings;

        public Adapter Adapter { get; private set; }

        public DeviceProfile Profile { get; private set; }

        public DeviceFeatures Features { get; private set; }

        public DeviceContext ImmediateContext { get; private set; }

        public BlendStateCollection BlendStates { get; private set; }

        public SamplerStateCollection SamplerStates { get; private set; }

        public RasterizerStateCollection RasterizerStates { get; private set; }

        public DepthStencilStateCollection DepthStencilStates { get; private set; }

        internal D3D11Device D3D11Device { get; private set; }

        public Device(Adapter adapter, DeviceSettings settings, DeviceProfile[] profiles)
        {
            Settings = settings;

            // アダプタの設定 (null 指定ならばデフォルト アダプタ)。
            Adapter = adapter ?? Adapter.DefaultAdapter;

            // デバイスの初期化。
            var creationFlags = Settings.GetD3D11DeviceCreationFlags();
            var featureLevels = ToD3DFeatureLevels(profiles);
            try
            {
                D3D11Device = new D3D11Device(Adapter.DXGIAdapter, creationFlags, featureLevels);
            }
            catch (SDXSharpDXException)
            {
                try
                {
                    D3D11Device = new D3D11Device(D3DDriverType.Warp, creationFlags, featureLevels);
                }
                catch (SDXSharpDXException)
                {
                    D3D11Device = new D3D11Device(D3DDriverType.Reference, creationFlags, featureLevels);
                }
            }

            // コンテキストの初期化。
            ImmediateContext = new DeviceContext(this, D3D11Device.ImmediateContext);

            // デバイス生成で選択されたプロファイルの記録。
            Profile = (DeviceProfile) D3D11Device.FeatureLevel;

            // デバイス特性の記録。
            Features = DeviceFeatures.Create(D3D11Device);

            // ブレンド ステートの初期化。
            BlendStates = new BlendStateCollection(this);
            BlendStates.Add(BlendState.Additive);
            BlendStates.Add(BlendState.AlphaBlend);
            BlendStates.Add(BlendState.NonPremultiplied);
            BlendStates.Add(BlendState.Opaque);

            // サンプラ ステートの初期化。
            SamplerStates = new SamplerStateCollection(this);
            SamplerStates.Add(SamplerState.AnisotropicClamp);
            SamplerStates.Add(SamplerState.AnisotropicWrap);
            SamplerStates.Add(SamplerState.LinearClamp);
            SamplerStates.Add(SamplerState.LinearWrap);
            SamplerStates.Add(SamplerState.PointClamp);
            SamplerStates.Add(SamplerState.PointWrap);

            // レンダ ステートの初期化。
            RasterizerStates = new RasterizerStateCollection(this);
            RasterizerStates.Add(RasterizerState.CullNone);
            RasterizerStates.Add(RasterizerState.CullFront);
            RasterizerStates.Add(RasterizerState.CullBack);

            // 深度ステンシル ステートの初期化。
            DepthStencilStates = new DepthStencilStateCollection(this);
            DepthStencilStates.Add(DepthStencilState.Default);
            DepthStencilStates.Add(DepthStencilState.DepthRead);
            DepthStencilStates.Add(DepthStencilState.None);
        }

        public DeviceContext CreateDeferredContext()
        {
            var d3d11DeviceContext = new D3D11DeviceContext(D3D11Device);
            return new DeviceContext(this, d3d11DeviceContext);
        }

        public int CheckMultiSampleQualityLevels(SurfaceFormat format, int sampleCount)
        {
            if (sampleCount <= 0) throw new ArgumentOutOfRangeException("sampleCount");

            if (sampleCount == 1) return 0;

            return D3D11Device.CheckMultisampleQualityLevels((DXGIFormat) format, sampleCount);
        }

        public int CheckMultiSampleQualityLevels(DepthFormat format, int sampleCount)
        {
            if (sampleCount <= 0) throw new ArgumentOutOfRangeException("sampleCount");

            if (sampleCount == 1) return 0;

            return D3D11Device.CheckMultisampleQualityLevels((DXGIFormat) format, sampleCount);
        }

        D3DFeatureLevel[] ToD3DFeatureLevels(DeviceProfile[] profiles)
        {
            if (profiles == null || profiles.Length == 0)
                return null;

            var result = new D3DFeatureLevel[profiles.Length];
            for (int i = 0; i < profiles.Length; i++)
                result[i] = (D3DFeatureLevel) profiles[i];

            return result;
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~Device()
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
            if (IsDisposed) return;

            if (Disposing != null)
                Disposing(this, EventArgs.Empty);

            if (disposing)
            {
                BlendStates.Dispose();
                SamplerStates.Dispose();
                RasterizerStates.Dispose();
                DepthStencilStates.Dispose();
                ImmediateContext.Dispose();
                D3D11Device.Dispose();
            }

            IsDisposed = true;
        }

        #endregion
    }
}
