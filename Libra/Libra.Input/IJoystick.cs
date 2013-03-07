#region Using

using System;

#endregion

namespace Libra.Input
{
    public interface IJoystick : IDisposable
    {
        bool Enabled { get; }

        string Name { get; }

        JoystickState GetState();
    }
}
