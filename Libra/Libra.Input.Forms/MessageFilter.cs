#region Using

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Libra.Input.Forms
{
    public sealed class MessageFilter : IMessageFilter, IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct TRACKMOUSEEVENT
        {
            public Int32 structureSize;

            public Int32 flags;
            
            public IntPtr trackWindowHandle;
            
            public Int32 hoverTime;
        }

        public enum WindowMessages : int
        {
            WM_GETDLGCODE       = 0x0087,
            WM_INPUT            = 0x00FF,
            WM_KEYDOWN          = 0x0100,
            WM_KEYUP            = 0x0101,
            WM_CHAR             = 0x0102,
            WM_UNICHAR          = 0x0109,
            WM_MOUSEMOVE        = 0x0200,
            WM_LBUTTONDOWN      = 0x0201,
            WM_LBUTTONUP        = 0x0202,
            WM_LBUTTONDBLCLK    = 0x0203,
            WM_RBUTTONDOWN      = 0x0204,
            WM_RBUTTONUP        = 0x0205,
            WM_RBUTTONDBLCLK    = 0x0206,
            WM_MBUTTONDOWN      = 0x0207,
            WM_MBUTTONUP        = 0x0208,
            WM_MBUTTONDBLCLK    = 0x0209,
            WM_MOUSEHWHEEL      = 0x020A,
            WM_XBUTTONDOWN      = 0x020B,
            WM_XBUTTONUP        = 0x020C,
            WM_XBUTTONDBLCLK    = 0x020D,
            WM_MOUSEHWHEEL_TILT = 0x020E,
            WM_MOUSELEAVE       = 0x02A3
        }

        public const int TME_LEAVE = 0x00000002;

        TRACKMOUSEEVENT mouseEventTrackData;

        bool trackingMouse;

        public MessageFilter(IntPtr windowHandle)
        {
            mouseEventTrackData = new TRACKMOUSEEVENT();
            mouseEventTrackData.structureSize = Marshal.SizeOf(this.mouseEventTrackData);
            mouseEventTrackData.flags = TME_LEAVE;
            mouseEventTrackData.trackWindowHandle = windowHandle;

            Application.AddMessageFilter(this);
        }

        [DllImport("user32")]
        public static extern int TrackMouseEvent(ref TRACKMOUSEEVENT eventTrack);

        public bool PreFilterMessage(ref Message message)
        {
            switch (message.Msg)
            {
                //case (int) UnsafeNativeMethods.WindowMessages.WM_KEYDOWN:
                //    {
                //        int virtualKeyCode = message.WParam.ToInt32();
                //        // bool repetition = (message.LParam.ToInt32() & WM_KEYDOWN_WASDOWN) != 0;
                //        OnKeyPressed((Keys) virtualKeyCode);

                //        if (virtualKeyCode == 17)
                //        {
                //            ctrlKeyDown = true;
                //        }
                //        if (virtualKeyCode == 18)
                //        {
                //            altKeyDown = true;
                //        }

                //        UnsafeNativeMethods.TranslateMessage(ref message);

                //        return true; // consumed!
                //    }
                //case (int) UnsafeNativeMethods.WindowMessages.WM_KEYUP:
                //    {
                //        int virtualKeyCode = message.WParam.ToInt32();
                //        OnKeyReleased((Keys) virtualKeyCode);

                //        // Workaround for strange behavior of ctrl and alt key combinations
                //        // Pressing & releasing both generates two keydowns but only one keyup.
                //        if (virtualKeyCode == 17)
                //        {
                //            ctrlKeyDown = false;
                //            if (altKeyDown)
                //            {
                //                OnKeyReleased((Keys) 18);
                //                altKeyDown = false;
                //            }
                //        }
                //        else if (virtualKeyCode == 18)
                //        {
                //            altKeyDown = false;
                //            if (ctrlKeyDown)
                //            {
                //                OnKeyReleased((Keys) 17);
                //                ctrlKeyDown = false;
                //            }
                //        }

                //        return true; // consumed!
                //    }

                //// Character has been entered on the keyboard
                //case (int) UnsafeNativeMethods.WindowMessages.WM_CHAR:
                //    {
                //        char character = (char) message.WParam.ToInt32();
                //        OnCharacterEntered(character);
                //        return true; // consumed!
                //    }

                case (int) WindowMessages.WM_MOUSEMOVE:
                    {
                        if (!trackingMouse)
                        {
                            int result = TrackMouseEvent(ref this.mouseEventTrackData);
                            trackingMouse = (result != 0);
                        }

                        short x = (short) (message.LParam.ToInt32() & 0xFFFF);
                        short y = (short) (message.LParam.ToInt32() >> 16);
                        OnMouseMoved((float) x, (float) y);
                        break;
                    }

                // Left mouse button pressed / released
                case (int) WindowMessages.WM_LBUTTONDOWN:
                case (int) WindowMessages.WM_LBUTTONDBLCLK:
                    {
                        OnMouseButtonPressed(MouseButtons.Left);
                        break;
                    }
                case (int) WindowMessages.WM_LBUTTONUP:
                    {
                        OnMouseButtonReleased(MouseButtons.Left);
                        break;
                    }

                // Right mouse button pressed / released
                case (int) WindowMessages.WM_RBUTTONDOWN:
                case (int) WindowMessages.WM_RBUTTONDBLCLK:
                    {
                        OnMouseButtonPressed(MouseButtons.Right);
                        break;
                    }
                case (int) WindowMessages.WM_RBUTTONUP:
                    {
                        OnMouseButtonReleased(MouseButtons.Right);
                        break;
                    }

                // Middle mouse button pressed / released
                case (int) WindowMessages.WM_MBUTTONDOWN:
                case (int) WindowMessages.WM_MBUTTONDBLCLK:
                    {
                        OnMouseButtonPressed(MouseButtons.Middle);
                        break;
                    }
                case (int) WindowMessages.WM_MBUTTONUP:
                    {
                        OnMouseButtonReleased(MouseButtons.Middle);
                        break;
                    }

                // Extended mouse button pressed / released
                case (int) WindowMessages.WM_XBUTTONDOWN:
                case (int) WindowMessages.WM_XBUTTONDBLCLK:
                    {
                        short button = (short) (message.WParam.ToInt32() >> 16);
                        if (button == 1)
                            OnMouseButtonPressed(MouseButtons.X1);
                        if (button == 2)
                            OnMouseButtonPressed(MouseButtons.X2);

                        break;
                    }
                case (int) WindowMessages.WM_XBUTTONUP:
                    {
                        short button = (short) (message.WParam.ToInt32() >> 16);
                        if (button == 1)
                            OnMouseButtonReleased(MouseButtons.X1);
                        if (button == 2)
                            OnMouseButtonReleased(MouseButtons.X2);

                        break;
                    }

                // Mouse wheel rotated
                case (int) WindowMessages.WM_MOUSEHWHEEL:
                    {
                        short ticks = (short) (message.WParam.ToInt32() >> 16);
                        OnMouseWheelRotated((float) ticks / 120.0f);
                        break;
                    }

                // Mouse has left the window's client area
                case (int) WindowMessages.WM_MOUSELEAVE:
                    {
                        OnMouseMoved(-1.0f, -1.0f);
                        this.trackingMouse = false;
                        break;
                    }
            }

            return false;
        }

        void OnMouseButtonPressed(MouseButtons buttons)
        {
            Console.WriteLine("OnMouseButtonPressed");
            //if (MouseButtonPressed != null)
            //{
            //    MouseButtonPressed(buttons);
            //}
        }

        void OnMouseButtonReleased(MouseButtons buttons)
        {
            Console.WriteLine("OnMouseButtonReleased");
            //if (MouseButtonReleased != null)
            //{
            //    MouseButtonReleased(buttons);
            //}
        }

        void OnMouseMoved(float x, float y)
        {
            //Console.WriteLine("OnMouseMoved");
            //if (MouseMoved != null)
            //{
            //    MouseMoved(x, y);
            //}
        }

        void OnMouseWheelRotated(float ticks)
        {
            Console.WriteLine("OnMouseWheelRotated");
            //if (MouseWheelRotated != null)
            //{
            //    MouseWheelRotated(ticks);
            //}
        }

        #region IDisposable

        bool disposed;

        ~MessageFilter()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                Application.RemoveMessageFilter(this);
            }

            disposed = true;
        }

        #endregion
    }
}
