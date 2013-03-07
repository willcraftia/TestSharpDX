#region Using

using System;

using DIDeviceEnumerationFlags = SharpDX.DirectInput.DeviceEnumerationFlags;
using DIDeviceInstance = SharpDX.DirectInput.DeviceInstance;
using DIDeviceType = SharpDX.DirectInput.DeviceType;
using DIDirectInput = SharpDX.DirectInput.DirectInput;

#endregion

namespace Libra.Input.SharpDX
{
    public sealed class SdxInputFactory : IInputFactory, IDisposable
    {
        DIDirectInput diDirectInput;

        public SdxInputFactory()
        {
            diDirectInput = new DIDirectInput();
        }

        public IKeyboard CreateKeyboard()
        {
            var devices = diDirectInput.GetDevices(DIDeviceType.Keyboard, DIDeviceEnumerationFlags.AllDevices);

            var device = (devices.Count != 0) ? devices[0] : null;
            return new SdxKeyboard(diDirectInput, device);
        }

        public IJoystick CreateJoystick()
        {
            var devices = diDirectInput.GetDevices(DIDeviceType.Joystick, DIDeviceEnumerationFlags.AllDevices);

            if (devices.Count == 0)
            {
                devices = diDirectInput.GetDevices(DIDeviceType.Gamepad, DIDeviceEnumerationFlags.AllDevices);
            }

            var device = (devices.Count != 0) ? devices[0] : null;
            return new SdxJoystick(diDirectInput, device);
        }

        #region IDisposable

        bool disposed;

        ~SdxInputFactory()
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
                diDirectInput.Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
