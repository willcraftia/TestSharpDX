#region Using

using System;

using D3D11DeviceCreationFlags = SharpDX.Direct3D11.DeviceCreationFlags;

#endregion

namespace Libra.Graphics
{
    public struct DeviceSettings
    {
        public bool SingleThreaded;

        public bool Debug;

        internal D3D11DeviceCreationFlags GetD3D11DeviceCreationFlags()
        {
            // TODO
            // 後で再考。

            var result = D3D11DeviceCreationFlags.None;
            
            if (SingleThreaded)
                result |= D3D11DeviceCreationFlags.SingleThreaded;

            if (Debug)
                result |= D3D11DeviceCreationFlags.Debug;

            return result;
        }
    }
}
