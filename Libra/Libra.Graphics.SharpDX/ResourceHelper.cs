#region Using

using System;

using D3D11CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags;
using D3D11ResourceUsage = SharpDX.Direct3D11.ResourceUsage;

#endregion

namespace Libra.Graphics.SharpDX
{
    public static class ResourceHelper
    {
        public static D3D11CpuAccessFlags GetD3D11CpuAccessFlags(D3D11ResourceUsage usage)
        {
            if (usage == D3D11ResourceUsage.Staging)
                return D3D11CpuAccessFlags.Read | D3D11CpuAccessFlags.Write;

            if (usage == D3D11ResourceUsage.Dynamic)
                return D3D11CpuAccessFlags.Write;

            return D3D11CpuAccessFlags.None;
        }
    }
}
