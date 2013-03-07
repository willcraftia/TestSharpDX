#region Using

using System;

using DIDeviceInstance = SharpDX.DirectInput.DeviceInstance;
using DIDirectInput = SharpDX.DirectInput.DirectInput;
using DIKey = SharpDX.DirectInput.Key;
using DIKeyboardUpdate = SharpDX.DirectInput.KeyboardUpdate;
using DIRawKeyboardState = SharpDX.DirectInput.RawKeyboardState;

#endregion

namespace Libra.Input.SharpDX
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// SharpDX の Keyboard クラスでは、KeyboardState をクラス型で扱っています。
    /// しかし、Libra では構造体で扱いたいため、
    /// この目的に合致するようにブリッジを実装しています。
    /// </remarks>
    public sealed class SdxKeyboard : IKeyboard
    {
        #region KeyboardBridge

        sealed class KeyboardBridge : global::SharpDX.DirectInput.CustomDevice<KeyboardStateBridge, DIRawKeyboardState, DIKeyboardUpdate>
        {
            public KeyboardBridge(DIDirectInput diDirectInput, Guid deviceGuid)
                : base(diDirectInput, deviceGuid)
            {
            }
        }

        #endregion

        #region KeyboardStateBridge

        sealed class KeyboardStateBridge : global::SharpDX.DirectInput.IDeviceState<DIRawKeyboardState, DIKeyboardUpdate>
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

        KeyboardBridge keyboardBridge;

        KeyboardStateBridge keyboardStateBridge;

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
                keyboardBridge = new KeyboardBridge(diDirectInput, diDevice.InstanceGuid);
                keyboardBridge.Acquire();
                keyboardStateBridge = new KeyboardStateBridge();
            }
        }

        public KeyboardState GetState()
        {
            if (!Enabled)
                return new KeyboardState();

            lock (this)
            {
                keyboardBridge.GetCurrentState(ref keyboardStateBridge);
                return keyboardStateBridge.State;
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
                if (keyboardBridge != null)
                    keyboardBridge.Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
