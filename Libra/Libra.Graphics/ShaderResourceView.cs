#region Using

using System;

using D3D11ShaderResourceView = SharpDX.Direct3D11.ShaderResourceView;

#endregion

namespace Libra.Graphics
{
    public sealed class ShaderResourceView : ResourceView
    {
        internal D3D11ShaderResourceView D3D11ShaderResourceView
        {
            get { return D3D11ResourceView as D3D11ShaderResourceView; }
        }

        public ShaderResourceView(Resource resource)
            : base(resource, CreateD3D11ResourceView(resource), false)
        {
        }

        static D3D11ShaderResourceView CreateD3D11ResourceView(Resource resource)
        {
            return new D3D11ShaderResourceView(resource.Device.D3D11Device, resource.D3D11Resource);
        }
    }
}
