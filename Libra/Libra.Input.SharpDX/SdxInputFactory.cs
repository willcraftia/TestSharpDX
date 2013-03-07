#region Using

using System;

#endregion

namespace Libra.Input.SharpDX
{
    public sealed class SdxInputFactory : IInputFactory
    {
        public IKeyboard CreateKeyboard()
        {
            return new SdxKeyboard();
        }
    }
}
