#region Using

using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

#endregion

namespace Libra.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InputPositionColor : IInputType, IEquatable<InputPositionColor>
    {
        public static readonly int SizeInBytes;

        static InputElement[] InputElements = { InputElement.SVPosition, InputElement.Color };

        public Vector3 Position;

        public Color Color;

        static InputPositionColor()
        {
            unsafe
            {
                SizeInBytes = sizeof(InputPositionColor);
            }
        }

        public InputPositionColor(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
        }

        InputElement[] IInputType.InputElements
        {
            get { return (InputElement[]) InputElements.Clone(); }
        }

        #region Equatable

        public static bool operator ==(InputPositionColor value1, InputPositionColor value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(InputPositionColor value1, InputPositionColor value2)
        {
            return !value1.Equals(value2);
        }

        public bool Equals(InputPositionColor other)
        {
            return Position == other.Position && Color == other.Color;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return Equals((InputPositionColor) obj);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Color.GetHashCode();
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return "[Position=" + Position + ", Color=" + Color + "]";
        }

        #endregion
    }
}
