#region Using

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Libra.Input.Forms
{
    public sealed class MessageFilter : IMessageFilter
    {
        [StructLayout(LayoutKind.Sequential)]
        struct MouseEvent
        {
            public Int32 structureSize;

            public Int32 flags;
            
            public IntPtr trackWindowHandle;
            
            public Int32 hoverTime;
        }

        enum WindowMessages : int
        {
            GETDLGCODE       = 0x0087,
            INPUT            = 0x00FF,
            KEYDOWN          = 0x0100,
            KEYUP            = 0x0101,
            CHAR             = 0x0102,
            UNICHAR          = 0x0109,
            MOUSEMOVE        = 0x0200,
            LBUTTONDOWN      = 0x0201,
            LBUTTONUP        = 0x0202,
            LBUTTONDBLCLK    = 0x0203,
            RBUTTONDOWN      = 0x0204,
            RBUTTONUP        = 0x0205,
            RBUTTONDBLCLK    = 0x0206,
            MBUTTONDOWN      = 0x0207,
            MBUTTONUP        = 0x0208,
            MBUTTONDBLCLK    = 0x0209,
            MOUSEHWHEEL      = 0x020A,
            XBUTTONDOWN      = 0x020B,
            XBUTTONUP        = 0x020C,
            XBUTTONDBLCLK    = 0x020D,
            MOUSEHWHEEL_TILT = 0x020E,
            MOUSELEAVE       = 0x02A3
        }

        const int TME_LEAVE = 0x00000002;

        MouseEvent mouseEvent;

        bool trackingMouse;

        public MessageFilter(IntPtr windowHandle)
        {
            mouseEvent = new MouseEvent();
            mouseEvent.structureSize = Marshal.SizeOf(this.mouseEvent);
            mouseEvent.flags = TME_LEAVE;
            mouseEvent.trackWindowHandle = windowHandle;
        }

        [DllImport("user32")]
        static extern int TrackMouseEvent(ref MouseEvent eventTrack);

        public bool PreFilterMessage(ref Message message)
        {
            switch (message.Msg)
            {
                case (int) WindowMessages.KEYDOWN:
                    {
                        int key = message.WParam.ToInt32();
                        FormKeyboard.Instance.State[(Keys) key] = KeyState.Down;
                        break;
                    }
                case (int) WindowMessages.KEYUP:
                    {
                        int key = message.WParam.ToInt32();
                        FormKeyboard.Instance.State[(Keys) key] = KeyState.Up;
                        break;
                    }

                case (int) WindowMessages.MOUSEMOVE:
                    {
                        if (!trackingMouse)
                        {
                            int result = TrackMouseEvent(ref mouseEvent);
                            trackingMouse = (result != 0);
                        }

                        short x = (short) (message.LParam.ToInt32() & 0xFFFF);
                        short y = (short) (message.LParam.ToInt32() >> 16);

                        FormMouse.Instance.State.X = x;
                        FormMouse.Instance.State.Y = y;
                        break;
                    }

                case (int) WindowMessages.LBUTTONDOWN:
                case (int) WindowMessages.LBUTTONDBLCLK:
                    {
                        FormMouse.Instance.State.LeftButton = ButtonState.Pressed;
                        break;
                    }
                case (int) WindowMessages.LBUTTONUP:
                    {
                        FormMouse.Instance.State.LeftButton = ButtonState.Released;
                        break;
                    }

                case (int) WindowMessages.RBUTTONDOWN:
                case (int) WindowMessages.RBUTTONDBLCLK:
                    {
                        FormMouse.Instance.State.RightButton = ButtonState.Pressed;
                        break;
                    }
                case (int) WindowMessages.RBUTTONUP:
                    {
                        FormMouse.Instance.State.RightButton = ButtonState.Released;
                        break;
                    }

                case (int) WindowMessages.MBUTTONDOWN:
                case (int) WindowMessages.MBUTTONDBLCLK:
                    {
                        FormMouse.Instance.State.MiddleButton = ButtonState.Pressed;
                        break;
                    }
                case (int) WindowMessages.MBUTTONUP:
                    {
                        FormMouse.Instance.State.MiddleButton = ButtonState.Released;
                        break;
                    }

                case (int) WindowMessages.XBUTTONDOWN:
                case (int) WindowMessages.XBUTTONDBLCLK:
                    {
                        short button = (short) (message.WParam.ToInt32() >> 16);
                        if (button == 1)
                        {
                            FormMouse.Instance.State.XButton1 = ButtonState.Pressed;
                        }
                        if (button == 2)
                        {
                            FormMouse.Instance.State.XButton2 = ButtonState.Pressed;
                        }

                        break;
                    }
                case (int) WindowMessages.XBUTTONUP:
                    {
                        short button = (short) (message.WParam.ToInt32() >> 16);
                        if (button == 1)
                        {
                            FormMouse.Instance.State.XButton1 = ButtonState.Released;
                        }
                        if (button == 2)
                        {
                            FormMouse.Instance.State.XButton2 = ButtonState.Released;
                        }

                        break;
                    }

                case (int) WindowMessages.MOUSEHWHEEL:
                    {
                        short ticks = (short) (message.WParam.ToInt32() >> 16);
                        FormMouse.Instance.State.ScrollWheelValue = ticks;
                        break;
                    }

                case (int) WindowMessages.MOUSELEAVE:
                    {
                        FormMouse.Instance.State.X = -1;
                        FormMouse.Instance.State.Y = -1;
                        this.trackingMouse = false;
                        break;
                    }
            }

            return false;
        }
    }
}
