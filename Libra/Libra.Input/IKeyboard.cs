﻿#region Using

using System;

#endregion

namespace Libra.Input
{
    public interface IKeyboard : IDisposable
    {
        bool Enabled { get; }

        string Name { get; }

        KeyboardState GetState();
    }
}
