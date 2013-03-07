#region Using

using System;

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
            public KeyboardBridge(DIDirectInput diDirectInput)
                : base(diDirectInput, SysKeyboardGuid)
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

        /// <summary>
        /// </summary>
        /// <remarks>
        /// dinput.h: GUID_SysKeyboard
        /// </remarks>
        static readonly Guid SysKeyboardGuid = new Guid(0x6F1D2B61, 0xD5A0, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00);

        DIDirectInput diDirectInput;

        KeyboardBridge keyboardBridge;

        KeyboardStateBridge keyboardStateBridge;

        public SdxKeyboard()
        {
            diDirectInput = new DIDirectInput();
            keyboardBridge = new KeyboardBridge(diDirectInput);
            keyboardBridge.Acquire();
            keyboardStateBridge = new KeyboardStateBridge();
        }

        public KeyboardState GetState()
        {
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
                keyboardBridge.Dispose();
                diDirectInput.Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
