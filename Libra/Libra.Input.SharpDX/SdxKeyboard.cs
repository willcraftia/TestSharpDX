#region Using

using System;

using DIDeviceEnumerationFlags = SharpDX.DirectInput.DeviceEnumerationFlags;
using DIDeviceInstance = SharpDX.DirectInput.DeviceInstance;
using DIDeviceType = SharpDX.DirectInput.DeviceType;
using DIDirectInput = SharpDX.DirectInput.DirectInput;
using DIKey = SharpDX.DirectInput.Key;
using DIKeyboardUpdate = SharpDX.DirectInput.KeyboardUpdate;
using DIRawKeyboardState = SharpDX.DirectInput.RawKeyboardState;

#endregion

namespace Libra.Input.SharpDX
{
    // TODO
    //
    // Dispose する必要あり。ただし、後でこのクラスそのものを破棄する。

    /// <summary>
    /// </summary>
    /// <remarks>
    /// SharpDX の Keyboard クラスでは、KeyboardState をクラス型で扱っています。
    /// しかし、Libra では構造体で扱いたいため、
    /// この目的に合致するようにブリッジを実装しています。
    /// </remarks>
    public sealed class SdxKeyboard : IKeyboard
    {
        #region Bridge

        sealed class Bridge : global::SharpDX.DirectInput.CustomDevice<StateBridge, DIRawKeyboardState, DIKeyboardUpdate>
        {
            public Bridge(DIDirectInput diDirectInput, Guid deviceGuid)
                : base(diDirectInput, deviceGuid)
            {
            }
        }

        #endregion

        #region StateBridge

        sealed class StateBridge : global::SharpDX.DirectInput.IDeviceState<DIRawKeyboardState, DIKeyboardUpdate>
        {
            public KeyboardState State;

            public void MarshalFrom(ref DIRawKeyboardState value)
            {
                State = new KeyboardState();

                unsafe
                {
                    var diUpdate = new DIKeyboardUpdate();
                    fixed (byte* pRawKeys = value.Keys)
                    {
                        for (int i = 0; i < 256; i++)
                        {
                            diUpdate.RawOffset = i;
                            diUpdate.Value = pRawKeys[i];

                            if (!diUpdate.IsPressed)
                                continue;
                            
                            var diKey = diUpdate.Key;
                            if (diKey == DIKey.Unknown)
                                continue;

                            State[(Keys) diKey] = KeyState.Down;
                        }
                    }
                }
            }

            // 不要。
            public void Update(DIKeyboardUpdate update) { throw new NotImplementedException(); }
        }

        #endregion

        DIDeviceInstance diDevice;

        Bridge bridge;

        StateBridge stateBridge;

        public bool Enabled { get; private set; }

        public string Name { get; private set; }

        public SdxKeyboard(DIDirectInput diDirectInput, DIDeviceInstance diDevice)
        {
            if (diDirectInput == null) throw new ArgumentNullException("diDirectInput");

            this.diDevice = diDevice;

            Enabled = (diDevice != null);
            Name = diDevice.ProductName;

            if (Enabled)
            {
                bridge = new Bridge(diDirectInput, diDevice.InstanceGuid);
                bridge.Acquire();
                stateBridge = new StateBridge();
            }
        }

        public static SdxKeyboard Create()
        {
            var diDirectInput = new DIDirectInput();

            var devices = diDirectInput.GetDevices(DIDeviceType.Keyboard, DIDeviceEnumerationFlags.AllDevices);

            var device = (devices.Count != 0) ? devices[0] : null;
            return new SdxKeyboard(diDirectInput, device);
        }

        public KeyboardState GetState()
        {
            if (!Enabled)
                return new KeyboardState();

            lock (this)
            {
                bridge.GetCurrentState(ref stateBridge);
                return stateBridge.State;
            }
        }

        #region IDisposable

        bool disposed;

        ~SdxKeyboard()
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
                if (bridge != null)
                    bridge.Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
