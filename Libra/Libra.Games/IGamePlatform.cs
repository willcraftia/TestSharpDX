#region Using

using System;

#endregion

namespace Libra.Games
{
    public interface IGamePlatform
    {
        event EventHandler Activated;

        event EventHandler Deactivated;

        event EventHandler Exiting;

        void CreateWindow();

        void Run(TickCallback tick);

        void Exit();
    }
}
