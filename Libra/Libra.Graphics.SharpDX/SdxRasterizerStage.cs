#region Using

using System;

using D3D11RasterizerStage = SharpDX.Direct3D11.RasterizerStage;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxRasterizerStage : IRasterizerStage
    {
        D3D11RasterizerStage d3d11RasterizerStage;

        RasterizerState rasterizerState;

        Viewport viewport;

        Rectangle scissorRectangle;

        public IDeviceContext DeviceContext { get; private set; }

        public RasterizerState RasterizerState
        {
            get { return rasterizerState; }
            set
            {
                if (rasterizerState == value) return;

                rasterizerState = value;

                var device = DeviceContext.Device as SdxDevice;
                if (rasterizerState == null)
                {
                    d3d11RasterizerStage.State = device.RasterizerStateManager[RasterizerState.CullBack];
                }
                else
                {
                    d3d11RasterizerStage.State = device.RasterizerStateManager[rasterizerState];
                }
            }
        }

        public Viewport Viewport
        {
            get { return viewport; }
            set
            {
                viewport = value;

                d3d11RasterizerStage.SetViewports(viewport.ToSDXViewportF());
            }
        }

        public Rectangle ScissorRectangle
        {
            get { return scissorRectangle; }
            set
            {
                scissorRectangle = value;

                d3d11RasterizerStage.SetScissorRectangles(scissorRectangle.ToSDXRectangle());
            }
        }

        internal SdxRasterizerStage(SdxDeviceContext context)
        {
            DeviceContext = context;

            d3d11RasterizerStage = (DeviceContext as SdxDeviceContext).D3D11DeviceContext.Rasterizer;
        }
    }
}
