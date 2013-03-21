﻿#region Using

using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

#endregion

namespace Libra.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InputPositionTexture : IInputType, IEquatable<InputPositionTexture>
    {
        public static readonly int SizeInBytes;

        static InputElement[] InputElements = { InputElement.SVPosition, InputElement.TexCoord };

        public Vector3 Position;

        public Vector2 TexCoord;

        static InputPositionTexture()
        {
            unsafe
            {
                SizeInBytes = sizeof(InputPositionTexture);
            }
        }

        public InputPositionTexture(Vector3 position, Vector2 texCoord)
        {
            Position = position;
            TexCoord = texCoord;
        }

        InputElement[] IInputType.InputElements
        {
            get { return (InputElement[]) InputElements.Clone(); }
        }

        #region Equatable

        public static bool operator ==(InputPositionTexture value1, InputPositionTexture value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(InputPositionTexture value1, InputPositionTexture value2)
        {
            return !value1.Equals(value2);
        }

        public bool Equals(InputPositionTexture other)
        {
            return Position == other.Position && TexCoord == other.TexCoord;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return Equals((InputPositionTexture) obj);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ TexCoord.GetHashCode();
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return "[Position=" + Position + ", TexCoord=" + TexCoord + "]";
        }

        #endregion
    }
}
