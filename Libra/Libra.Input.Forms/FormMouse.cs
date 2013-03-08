#region Using

using System;

#endregion

namespace Libra.Input.Forms
{
    public sealed class FormMouse : IMouse
    {
        public static readonly FormMouse Instance = new FormMouse();

        FormMouse() { }

        public MouseState State;

        public MouseState GetState()
        {
            return State;
        }
    }
}
