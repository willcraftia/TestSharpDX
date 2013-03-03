#region Using

using System;

using D3D11RasterizerStage = SharpDX.Direct3D11.RasterizerStage;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxRasterizerStage : RasterizerStage
    {
        public SdxDevice Device { get; private set; }

        public D3D11RasterizerStage D3D11RasterizerStage { get; private set; }

        public SdxRasterizerStage(SdxDevice device, D3D11RasterizerStage d3d11RasterizerStage)
        {
            if (device == null) throw new ArgumentNullException("device");
            if (d3d11RasterizerStage == null) throw new ArgumentNullException("d3d11RasterizerStage");

            Device = device;
            D3D11RasterizerStage = d3d11RasterizerStage;
        }

        protected override void OnRasterizerStateChanged()
        {
            if (RasterizerState == null)
            {
                D3D11RasterizerStage.State = null;
            }
            else
            {
                D3D11RasterizerStage.State = Device.RasterizerStateManager[RasterizerState];
            }
        }

        protected override void OnViewportChanged()
        {
            D3D11RasterizerStage.SetViewports(Viewport.ToSDXViewportF());
        }

        protected override void OnScissorRectangleChanged()
        {
            D3D11RasterizerStage.SetScissorRectangles(ScissorRectangle.ToSDXRectangle());
        }
    }
}
