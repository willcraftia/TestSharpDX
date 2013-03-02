#region Using

using System;

using D3D11Buffer = SharpDX.Direct3D11.Buffer;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxConstantBuffer : SdxBuffer, IConstantBuffer
    {
        public SdxConstantBuffer(D3D11Buffer d3d11Buffer)
            : base(d3d11Buffer)
        {
        }
    }
}
