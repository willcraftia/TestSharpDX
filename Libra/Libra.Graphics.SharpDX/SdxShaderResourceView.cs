#region Using

using System;

using D3D11ShaderResourceView = SharpDX.Direct3D11.ShaderResourceView;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxShaderResourceView : SdxResourceView, IShaderResourceView
    {
        public D3D11ShaderResourceView D3D11ShaderResourceView
        {
            get { return D3D11ResourceView as D3D11ShaderResourceView; }
        }

        public SdxShaderResourceView(
            D3D11ShaderResourceView d3d11ShaderResourceView,
            SdxResource resource,
            bool resourceResponsibility = false)
            : base(d3d11ShaderResourceView, resource, false)
        {
        }
    }
}
