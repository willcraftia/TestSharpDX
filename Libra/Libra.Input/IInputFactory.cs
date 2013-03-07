#region Using

using System;

#endregion

namespace Libra.Input
{
    public interface IInputFactory
    {
        IKeyboard CreateKeyboard();

        IJoystick CreateJoystick();
    }
}
