#region Using

using System;

using D3D11RasterizerStage = SharpDX.Direct3D11.RasterizerStage;

#endregion

namespace Libra.Graphics
{
    public sealed class RasterizerStage
    {
        D3D11RasterizerStage d3d11RasterizerStage;

        RasterizerState rasterizerState;

        Viewport viewport;

        Rectangle scissorRectangle;

        public DeviceContext DeviceContext { get; private set; }

        public RasterizerState RasterizerState
        {
            get { return rasterizerState; }
            set
            {
                if (rasterizerState == value) return;

                rasterizerState = value;

                if (rasterizerState == null)
                {
                    d3d11RasterizerStage.State = DeviceContext.Device.RasterizerStates[RasterizerState.CullBack];
                }
                else
                {
                    d3d11RasterizerStage.State = DeviceContext.Device.RasterizerStates[rasterizerState];
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

        internal RasterizerStage(DeviceContext context)
        {
            DeviceContext = context;

            d3d11RasterizerStage = DeviceContext.D3D11DeviceContext.Rasterizer;
        }
    }
}
