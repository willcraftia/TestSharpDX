#region Using

using System;

using D3D11BindFlags = SharpDX.Direct3D11.BindFlags;
using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11BufferDescription = SharpDX.Direct3D11.BufferDescription;
using D3D11ResourceOptionFlags = SharpDX.Direct3D11.ResourceOptionFlags;
using D3D11ResourceUsage = SharpDX.Direct3D11.ResourceUsage;

#endregion

namespace Libra.Graphics.SharpDX
{
    public class SdxBuffer : SdxResource, IBuffer
    {
        internal D3D11Buffer D3D11Buffer
        {
            get { return D3D11Resource as D3D11Buffer; }
        }

        public SdxBuffer(D3D11Buffer d3d11Buffer)
            : base(d3d11Buffer)
        {
        }
    }
}
