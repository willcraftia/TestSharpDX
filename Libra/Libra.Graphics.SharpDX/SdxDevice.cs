#region Using

using System;
using System.Collections.Generic;

using D3D11BindFlags = SharpDX.Direct3D11.BindFlags;
using D3D11Blend = SharpDX.Direct3D11.BlendOption;
using D3D11BlendOperation = SharpDX.Direct3D11.BlendOperation;
using D3D11BlendState = SharpDX.Direct3D11.BlendState;
using D3D11BlendStateDescription = SharpDX.Direct3D11.BlendStateDescription;
using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11BufferDescription = SharpDX.Direct3D11.BufferDescription;
using D3D11ColorWriteMaskFlags = SharpDX.Direct3D11.ColorWriteMaskFlags;
using D3D11Comparison = SharpDX.Direct3D11.Comparison;
using D3D11CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags;
using D3D11CullMode = SharpDX.Direct3D11.CullMode;
using D3D11DepthStencilState = SharpDX.Direct3D11.DepthStencilState;
using D3D11DepthStencilStateDescription = SharpDX.Direct3D11.DepthStencilStateDescription;
using D3D11DepthStencilView = SharpDX.Direct3D11.DepthStencilView;
using D3D11DepthStencilViewDescription = SharpDX.Direct3D11.DepthStencilViewDescription;
using D3D11DepthStencilViewDimension = SharpDX.Direct3D11.DepthStencilViewDimension;
using D3D11DepthStencilViewFlags = SharpDX.Direct3D11.DepthStencilViewFlags;
using D3D11DepthWriteMask = SharpDX.Direct3D11.DepthWriteMask;
using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11DeviceChild = SharpDX.Direct3D11.DeviceChild;
using D3D11DeviceContext = SharpDX.Direct3D11.DeviceContext;
using D3D11DeviceCreationFlags = SharpDX.Direct3D11.DeviceCreationFlags;
using D3D11Feature = SharpDX.Direct3D11.Feature;
using D3D11FillMode = SharpDX.Direct3D11.FillMode;
using D3D11Filter = SharpDX.Direct3D11.Filter;
using D3D11InputClassification = SharpDX.Direct3D11.InputClassification;
using D3D11InputElement = SharpDX.Direct3D11.InputElement;
using D3D11InputLayout = SharpDX.Direct3D11.InputLayout;
using D3D11PixelShader = SharpDX.Direct3D11.PixelShader;
using D3D11RasterizerState = SharpDX.Direct3D11.RasterizerState;
using D3D11RasterizerStateDescription = SharpDX.Direct3D11.RasterizerStateDescription;
using D3D11RenderTargetView = SharpDX.Direct3D11.RenderTargetView;
using D3D11RenderTargetViewDescription = SharpDX.Direct3D11.RenderTargetViewDescription;
using D3D11RenderTargetViewDimension = SharpDX.Direct3D11.RenderTargetViewDimension;
using D3D11ResourceUsage = SharpDX.Direct3D11.ResourceUsage;
using D3D11ResourceOptionFlags = SharpDX.Direct3D11.ResourceOptionFlags;
using D3D11SamplerState = SharpDX.Direct3D11.SamplerState;
using D3D11SamplerStateDescription = SharpDX.Direct3D11.SamplerStateDescription;
using D3D11ShaderResourceView = SharpDX.Direct3D11.ShaderResourceView;
using D3D11StencilOperation = SharpDX.Direct3D11.StencilOperation;
using D3D11Texture2D = SharpDX.Direct3D11.Texture2D;
using D3D11Texture2DDescription = SharpDX.Direct3D11.Texture2DDescription;
using D3D11TextureAddressMode = SharpDX.Direct3D11.TextureAddressMode;
using D3D11VertexShader = SharpDX.Direct3D11.VertexShader;
using D3DDriverType = SharpDX.Direct3D.DriverType;
using D3DFeatureLevel = SharpDX.Direct3D.FeatureLevel;
using DXGIFormat = SharpDX.DXGI.Format;
using SDXSharpDXException = SharpDX.SharpDXException;

#endregion

namespace Libra.Graphics.SharpDX
{
    using BlendStateManager = SdxStateManager<BlendState, D3D11BlendState>;
    using SamplerStateManager = SdxStateManager<SamplerState, D3D11SamplerState>;
    using RasterizerStateManager = SdxStateManager<RasterizerState, D3D11RasterizerState>;
    using DepthStencilStateManager = SdxStateManager<DepthStencilState, D3D11DepthStencilState>;

    public sealed class SdxDevice : IDevice
    {
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

        public IAdapter Adapter { get; private set; }

        public DeviceProfile Profile { get; private set; }

        public SdxDeviceFeatures Features { get; private set; }

        public IDeviceContext ImmediateContext { get; private set; }

        public D3D11Device D3D11Device { get; private set; }

        public BlendStateManager BlendStateManager { get; private set; }

        public SamplerStateManager SamplerStateManager { get; private set; }

        public RasterizerStateManager RasterizerStateManager { get; private set; }

        public DepthStencilStateManager DepthStencilStateManager { get; private set; }

        public SdxDevice(SdxAdapter adapter, DeviceSettings settings, DeviceProfile[] profiles)
        {
            Adapter = adapter;
            Settings = settings;

            // デバイスの初期化。
            var creationFlags = ResolveD3D11DeviceCreationFlags(settings);
            var featureLevels = ToD3DFeatureLevels(profiles);
            if (Adapter != null)
            {
                try
                {
                    var dxgiAdapter = (Adapter as SdxAdapter).DXGIAdapter;
                    D3D11Device = new D3D11Device(dxgiAdapter, creationFlags, featureLevels);
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
            }
            else
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
            ImmediateContext = new SdxDeviceContext(this, D3D11Device.ImmediateContext);

            // デバイス生成で選択されたプロファイルの記録。
            Profile = (DeviceProfile) D3D11Device.FeatureLevel;

            // デバイス特性の記録。
            Features = SdxDeviceFeatures.Create(D3D11Device);

            // ブレンド ステートの初期化。
            BlendStateManager = new BlendStateManager(this, CreateD3D11BlendState);
            BlendStateManager.Register(BlendState.Additive);
            BlendStateManager.Register(BlendState.AlphaBlend);
            BlendStateManager.Register(BlendState.NonPremultiplied);
            BlendStateManager.Register(BlendState.Opaque);

            // サンプラ ステートの初期化。
            SamplerStateManager = new SamplerStateManager(this, CreateD3D11SamplerState);
            SamplerStateManager.Register(SamplerState.AnisotropicClamp);
            SamplerStateManager.Register(SamplerState.AnisotropicWrap);
            SamplerStateManager.Register(SamplerState.LinearClamp);
            SamplerStateManager.Register(SamplerState.LinearWrap);
            SamplerStateManager.Register(SamplerState.PointClamp);
            SamplerStateManager.Register(SamplerState.PointWrap);

            // レンダ ステートの初期化。
            RasterizerStateManager = new RasterizerStateManager(this, CreateD3D11RasterizerState);
            RasterizerStateManager.Register(RasterizerState.CullNone);
            RasterizerStateManager.Register(RasterizerState.CullFront);
            RasterizerStateManager.Register(RasterizerState.CullBack);

            // 深度ステンシル ステートの初期化。
            DepthStencilStateManager = new DepthStencilStateManager(this, CreateD3D11DepthStencilState);
            DepthStencilStateManager.Register(DepthStencilState.Default);
            DepthStencilStateManager.Register(DepthStencilState.DepthRead);
            DepthStencilStateManager.Register(DepthStencilState.None);
        }

        public IDeviceContext CreateDeferredContext()
        {
            var d3d11DeviceContext = new D3D11DeviceContext(D3D11Device);
            return new SdxDeviceContext(this, d3d11DeviceContext);
        }

        public IVertexShader CreateVertexShader(byte[] shaderBytecode)
        {
            if (shaderBytecode == null) throw new ArgumentNullException("shaderBytecode");

            var d3d11VertexShader = new D3D11VertexShader(D3D11Device, shaderBytecode);

            return new SdxVertexShader(d3d11VertexShader);
        }

        public IPixelShader CreatePixelShader(byte[] shaderBytecode)
        {
            if (shaderBytecode == null) throw new ArgumentNullException("shaderBytecode");

            var d3d11PixelShader = new D3D11PixelShader(D3D11Device, shaderBytecode);

            return new SdxPixelShader(d3d11PixelShader);
        }

        public IConstantBuffer CreateConstantBuffer(
            int sizeInBytes,
            ResourceUsage usage = ResourceUsage.Default)
        {
            if (sizeInBytes <= 0) throw new ArgumentOutOfRangeException("sizeInBytes");

            // 初期データ無しの構築であるため Immutable は禁止。
            if (usage == ResourceUsage.Immutable)
                throw new ArgumentException("Usage must not be immutable.", "usage");

            var d3d11ResuorceUsage = (D3D11ResourceUsage) usage;
            var d3d11BindFlags = D3D11BindFlags.ConstantBuffer;

            var d3d11Buffer = CreateD3D11Buffer(sizeInBytes, d3d11ResuorceUsage, d3d11BindFlags);

            return new SdxConstantBuffer(d3d11Buffer);
        }

        public IConstantBuffer CreateConstantBuffer<T>(
            ResourceUsage usage = ResourceUsage.Default) where T : struct
        {
            return CreateConstantBuffer(SdxUtilities.SizeOf<T>(), usage);
        }

        public IVertexBuffer CreateVertexBuffer(
            int sizeInBytes,
            ResourceUsage usage = ResourceUsage.Default)
        {
            if (sizeInBytes <= 0) throw new ArgumentOutOfRangeException("sizeInBytes");

            // 初期データ無しの構築であるため Immutable は禁止。
            if (usage == ResourceUsage.Immutable)
                throw new ArgumentException("Usage must not be immutable.", "usage");

            var d3d11ResuorceUsage = (D3D11ResourceUsage) usage;
            var d3d11BindFlags = D3D11BindFlags.VertexBuffer;

            var d3d11Buffer = CreateD3D11Buffer(sizeInBytes, d3d11ResuorceUsage, d3d11BindFlags);

            return new SdxVertexBuffer(d3d11Buffer);
        }

        public IVertexBuffer CreateVertexBuffer<T>(
            T[] data,
            ResourceUsage usage = ResourceUsage.Immutable) where T : struct
        {
            if (data == null) throw new ArgumentNullException("data");

            var d3d11ResuorceUsage = (D3D11ResourceUsage) usage;
            var d3d11BindFlags = D3D11BindFlags.ConstantBuffer;

            var d3d11Buffer = CreateD3D11Buffer(data, d3d11ResuorceUsage, d3d11BindFlags);

            return new SdxVertexBuffer(d3d11Buffer);
        }

        public IInputLayout CreateInputLayout(byte[] shaderBytecode, params InputElement[] elements)
        {
            int inputStride;
            var d3d11InputLayout = CreateD3D11InputLayout(shaderBytecode, elements, out inputStride);

            return new SdxInputLayout(d3d11InputLayout, inputStride);
        }

        public IInputLayout CreateInputLayout<T>(byte[] shaderBytecode) where T : IInputType
        {
            var dummyObject = Activator.CreateInstance(typeof(T)) as IInputType;
            var elements = dummyObject.GetInputElements();

            return CreateInputLayout(shaderBytecode, elements);
        }

        public ITexture2D CreateTexture2D(
            int width,
            int height,
            bool mipMap = false,
            SurfaceFormat format = SurfaceFormat.Color,
            int multiSampleCount = 1,
            ResourceUsage usage = ResourceUsage.Default)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException("width");
            if (height <= 0) throw new ArgumentOutOfRangeException("height");
            if (multiSampleCount < 1) throw new ArgumentOutOfRangeException("multiSampleCount");

            // 初期データ無しの構築であるため Immutable は禁止。
            if (usage == ResourceUsage.Immutable)
                throw new ArgumentException("Usage must not be immutable.", "usage");

            var dxgiFormat = (DXGIFormat) format;
            var multiSampleQuality = D3D11Device.CheckMultisampleQualityLevels(dxgiFormat, multiSampleCount);
            var d3d11ResuorceUsage = (D3D11ResourceUsage) usage;
            var d3d11BindFlags = D3D11BindFlags.ShaderResource;

            var d3d11Texture2D = CreateD3D11Texture2D(
                width, height, mipMap, dxgiFormat, multiSampleCount, multiSampleQuality, d3d11ResuorceUsage, d3d11BindFlags);

            return new SdxTexture2D(d3d11Texture2D);
        }

        public IDepthStencil CreateDepthStencil(
            int width,
            int height,
            DepthFormat format = DepthFormat.Depth24Stencil8,
            int multiSampleCount = 1,
            int multiSampleQuality = 0)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException("width");
            if (height <= 0) throw new ArgumentOutOfRangeException("height");
            if (multiSampleCount < 1) throw new ArgumentOutOfRangeException("multiSampleCount");
            if (multiSampleQuality < 0) throw new ArgumentOutOfRangeException("multiSampleQuality");

            var dxgiFormat = (DXGIFormat) format;
            var d3d11ResuorceUsage = D3D11ResourceUsage.Default;
            var d3d11BindFlags = D3D11BindFlags.DepthStencil;

            var d3d11Texture2D = CreateD3D11Texture2D(
                width, height, false, dxgiFormat, multiSampleCount, multiSampleQuality, d3d11ResuorceUsage, d3d11BindFlags);

            return new SdxDepthStencil(d3d11Texture2D);
        }

        public IRenderTarget CreateRenderTarget(
            int width,
            int height,
            bool mipMap = false,
            SurfaceFormat format = SurfaceFormat.Color,
            int multiSampleCount = 1,
            ResourceUsage resourceUsage = ResourceUsage.Default,
            RenderTargetUsage renderTargetUsage = RenderTargetUsage.Discard)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException("width");
            if (height <= 0) throw new ArgumentOutOfRangeException("height");
            if (multiSampleCount < 1) throw new ArgumentOutOfRangeException("multiSampleCount");

            if (resourceUsage != ResourceUsage.Default && resourceUsage != ResourceUsage.Staging)
                throw new ArgumentException("ResourceUsage.Default or Staging required.", "usage");

            var dxgiFormat = (DXGIFormat) format;
            var multiSampleQuality = D3D11Device.CheckMultisampleQualityLevels(dxgiFormat, multiSampleCount);
            var d3d11ResuorceUsage = (D3D11ResourceUsage) resourceUsage;
            var d3d11BindFlags = D3D11BindFlags.ShaderResource | D3D11BindFlags.RenderTarget;

            var d3d11Texture2D = CreateD3D11Texture2D(
                width, height, mipMap, dxgiFormat, multiSampleCount, multiSampleQuality, d3d11ResuorceUsage, d3d11BindFlags);

            return new SdxRenderTarget(d3d11Texture2D, renderTargetUsage);
        }

        public IRenderTarget CreateRenderTarget(
            int width,
            int height,
            bool mipMap = false,
            SurfaceFormat format = SurfaceFormat.Color,
            DepthFormat depthFormat = DepthFormat.None,
            int multiSampleCount = 1,
            ResourceUsage resourceUsage = ResourceUsage.Default,
            RenderTargetUsage renderTargetUsage = RenderTargetUsage.Discard)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException("width");
            if (height <= 0) throw new ArgumentOutOfRangeException("height");
            if (multiSampleCount < 1) throw new ArgumentOutOfRangeException("multiSampleCount");

            if (resourceUsage != ResourceUsage.Default && resourceUsage != ResourceUsage.Staging)
                throw new ArgumentException("ResourceUsage.Default or Staging required.", "usage");

            var dxgiFormat = (DXGIFormat) format;
            var multiSampleQuality = D3D11Device.CheckMultisampleQualityLevels(dxgiFormat, multiSampleCount);
            var d3d11ResuorceUsage = (D3D11ResourceUsage) resourceUsage;
            var d3d11BindFlags = D3D11BindFlags.ShaderResource | D3D11BindFlags.RenderTarget;

            var d3d11Texture2D = CreateD3D11Texture2D(
                width, height, mipMap, dxgiFormat, multiSampleCount, multiSampleQuality, d3d11ResuorceUsage, d3d11BindFlags);

            IDepthStencil depthStencil = null;
            if (depthFormat != DepthFormat.None)
            {
                depthStencil = CreateDepthStencil(width, height, depthFormat, multiSampleCount, multiSampleQuality);
            }

            return new SdxRenderTarget(d3d11Texture2D, renderTargetUsage, depthStencil);
        }

        public IShaderResourceView CreateShaderResourceView(ITexture2D texture2D)
        {
            if (texture2D == null) throw new ArgumentNullException("texture2D");

            var sdxTexture2D = texture2D as SdxTexture2D;
            var d3d11Resource = sdxTexture2D.D3D11Resource;

            var d3d11ShaderResourceView = new D3D11ShaderResourceView(D3D11Device, d3d11Resource);

            return new SdxShaderResourceView(d3d11ShaderResourceView, sdxTexture2D);
        }

        public IDepthStencilView CreateDepthStencilView(IDepthStencil depthStencil)
        {
            if (depthStencil == null) throw new ArgumentNullException("depthStencil");

            var sdxDepthStencil = depthStencil as SdxDepthStencil;
            var d3d11Texture2D = sdxDepthStencil.D3D11Texture2D;

            var d3d11DepthStencilView = CreateD3D11DepthStencilView(d3d11Texture2D);

            return new SdxDepthStencilView(d3d11DepthStencilView, sdxDepthStencil);
        }

        public IRenderTargetView CreateRenderTargetView(IRenderTarget renderTarget)
        {
            if (renderTarget == null) throw new ArgumentNullException("renderTarget");

            var sdxRenderTarget = renderTarget as SdxRenderTarget;
            var d3d11Texture2D = sdxRenderTarget.D3D11Texture2D;

            var d3d11RenderTargetView = CreateD3D11RenderTargetView(d3d11Texture2D);

            IDepthStencilView depthStencilView = null;
            if (renderTarget.DepthStencil != null)
            {
                depthStencilView = CreateDepthStencilView(renderTarget.DepthStencil);
            }

            return new SdxRenderTargetView(d3d11RenderTargetView, sdxRenderTarget, false, depthStencilView);
        }

        public int CheckMultiSampleQualityLevels(SurfaceFormat format, int sampleCount)
        {
            if (sampleCount < 1) throw new ArgumentOutOfRangeException("sampleCount");

            if (sampleCount == 1) return 0;

            return D3D11Device.CheckMultisampleQualityLevels((DXGIFormat) format, sampleCount);
        }

        public int CheckMultiSampleQualityLevels(DepthFormat format, int sampleCount)
        {
            if (sampleCount < 1) throw new ArgumentOutOfRangeException("sampleCount");

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

        D3D11BlendState CreateD3D11BlendState(BlendState state)
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

            return new D3D11BlendState(D3D11Device, description);
        }

        D3D11SamplerState CreateD3D11SamplerState(SamplerState state)
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

            return new D3D11SamplerState(D3D11Device, description);
        }

        D3D11RasterizerState CreateD3D11RasterizerState(RasterizerState state)
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

            return new D3D11RasterizerState(D3D11Device, description);
        }

        D3D11DepthStencilState CreateD3D11DepthStencilState(DepthStencilState state)
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

            return new D3D11DepthStencilState(D3D11Device, description);
        }

        D3D11InputLayout CreateD3D11InputLayout(byte[] shaderBytecode, InputElement[] elements, out int inputStride)
        {
            inputStride = 0;

            var d3d11InputElements = new D3D11InputElement[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                var d3d11InputClassification = D3D11InputClassification.PerVertexData;
                if (elements[i].PerInstance)
                    d3d11InputClassification = D3D11InputClassification.PerInstanceData;

                d3d11InputElements[i] = new D3D11InputElement
                {
                    SemanticName = elements[i].SemanticName,
                    SemanticIndex = elements[i].SemanticIndex,
                    Format = (DXGIFormat) elements[i].Format,
                    Slot = elements[i].InputSlot,
                    AlignedByteOffset = elements[i].AlignedByteOffset,
                    Classification = d3d11InputClassification,
                    InstanceDataStepRate = elements[i].InstanceDataStepRate
                };

                inputStride += FormatHelper.SizeOfInBytes(elements[i].Format);
            }

            // シグネチャ生成無しでも入力レイアウトを生成可能。
            // このため、シグネチャを用いるならば、
            // クラス外部でこれを生成し、引数 shaderBytecode に指定すれば良い。
            return new D3D11InputLayout(D3D11Device, shaderBytecode, d3d11InputElements);
        }

        D3D11Buffer CreateD3D11Buffer(
            int sizeInBytes,
            D3D11ResourceUsage d3d11ResuorceUsage,
            D3D11BindFlags d3d11BindFlags)
        {
            var description = new D3D11BufferDescription
            {
                SizeInBytes = sizeInBytes,
                Usage = d3d11ResuorceUsage,
                BindFlags = d3d11BindFlags,
                CpuAccessFlags = ResolveD3D11CpuAccessFlags(d3d11ResuorceUsage),
                OptionFlags = D3D11ResourceOptionFlags.None,
                StructureByteStride = 0
            };

            return new D3D11Buffer(D3D11Device, description);
        }

        D3D11Buffer CreateD3D11Buffer<T>(
            T[] data,
            D3D11ResourceUsage d3d11ResuorceUsage,
            D3D11BindFlags d3d11BindFlags) where T : struct
        {
            // 「要素の構造体のサイズ」×「配列サイズ」がバッファのサイズ。
            var sizeInBytes = SdxUtilities.SizeOf<T>() * data.Length;

            var description = new D3D11BufferDescription
            {
                SizeInBytes = sizeInBytes,
                Usage = d3d11ResuorceUsage,
                BindFlags = d3d11BindFlags,
                CpuAccessFlags = ResolveD3D11CpuAccessFlags(d3d11ResuorceUsage),
                OptionFlags = D3D11ResourceOptionFlags.None,
                StructureByteStride = 0
            };

            return D3D11Buffer.Create<T>(D3D11Device, data, description);
        }

        D3D11Texture2D CreateD3D11Texture2D(
            int width,
            int height,
            bool mipMap,
            DXGIFormat dxgiFormat,
            int multiSampleCount,
            int multiSampleQuality,
            D3D11ResourceUsage d3d11ResuorceUsage,
            D3D11BindFlags bindFlags)
        {
            // 初期データ無しの構築であるため Immutable は禁止。
            if (d3d11ResuorceUsage == D3D11ResourceUsage.Immutable)
                throw new ArgumentException("Usage must not be immutable.", "d3d11ResuorceUsage");

            var description = new D3D11Texture2DDescription
            {
                Width = width,
                Height = height,
                MipLevels = (mipMap) ? 0 : 1,
                ArraySize = 1,
                Format = dxgiFormat,
                SampleDescription =
                {
                    Count = multiSampleCount,
                    //Quality = D3D11Device.CheckMultisampleQualityLevels(dxgiFormat, multiSampleCount)
                    Quality = multiSampleQuality
                },
                Usage = d3d11ResuorceUsage,
                BindFlags = bindFlags,
                CpuAccessFlags = ResolveD3D11CpuAccessFlags(d3d11ResuorceUsage),
                OptionFlags = (mipMap) ? D3D11ResourceOptionFlags.GenerateMipMaps : D3D11ResourceOptionFlags.None
            };

            return new D3D11Texture2D(D3D11Device, description);
        }

        D3D11DepthStencilView CreateD3D11DepthStencilView(D3D11Texture2D d3d11Texture2D)
        {
            var d3d11Texture2DDescription = d3d11Texture2D.Description;

            var description = new D3D11DepthStencilViewDescription
            {
                Format = d3d11Texture2DDescription.Format,
                Flags = D3D11DepthStencilViewFlags.None,
                Texture2D =
                {
                    MipSlice = 0
                }
            };

            if (1 < d3d11Texture2DDescription.SampleDescription.Count)
            {
                description.Dimension = D3D11DepthStencilViewDimension.Texture2DMultisampled;
            }
            else
            {
                description.Dimension = D3D11DepthStencilViewDimension.Texture2D;
            }

            return new D3D11DepthStencilView(D3D11Device, d3d11Texture2D, description);
        }

        D3D11RenderTargetView CreateD3D11RenderTargetView(D3D11Texture2D d3d11Texture2D)
        {
            var d3d11Texture2DDescription = d3d11Texture2D.Description;

            var description = new D3D11RenderTargetViewDescription
            {
                Format = d3d11Texture2DDescription.Format,
                Texture2D =
                {
                    MipSlice = 0
                }
            };

            if (1 < d3d11Texture2DDescription.SampleDescription.Count)
            {
                description.Dimension = D3D11RenderTargetViewDimension.Texture2DMultisampled;
            }
            else
            {
                description.Dimension = D3D11RenderTargetViewDimension.Texture2D;
            }

            return new D3D11RenderTargetView(D3D11Device, d3d11Texture2D, description);
        }

        D3D11CpuAccessFlags ResolveD3D11CpuAccessFlags(D3D11ResourceUsage usage)
        {
            if (usage == D3D11ResourceUsage.Staging)
                return D3D11CpuAccessFlags.Read | D3D11CpuAccessFlags.Write;

            if (usage == D3D11ResourceUsage.Dynamic)
                return D3D11CpuAccessFlags.Write;

            return D3D11CpuAccessFlags.None;
        }

        D3D11DeviceCreationFlags ResolveD3D11DeviceCreationFlags(DeviceSettings settings)
        {
            // TODO
            // 後で再考。

            var result = D3D11DeviceCreationFlags.None;

            if (settings.SingleThreaded)
                result |= D3D11DeviceCreationFlags.SingleThreaded;

            if (settings.Debug)
                result |= D3D11DeviceCreationFlags.Debug;

            return result;
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~SdxDevice()
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
                BlendStateManager.Dispose();
                SamplerStateManager.Dispose();
                RasterizerStateManager.Dispose();
                DepthStencilStateManager.Dispose();
                ImmediateContext.Dispose();
                D3D11Device.Dispose();
            }

            IsDisposed = true;
        }

        #endregion
    }
}
