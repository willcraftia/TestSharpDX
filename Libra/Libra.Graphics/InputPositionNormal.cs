#region Using

using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

#endregion

namespace Libra.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InputPositionNormal : IInputType, IEquatable<InputPositionNormal>
    {
        public static readonly int SizeInBytes;

        static ReadOnlyCollection<InputElement> InputElements = new ReadOnlyCollection<InputElement>(
            new [] { InputElement.SVPosition, InputElement.Normal });

        public Vector3 Position;

        public Vector3 Normal;

        static InputPositionNormal()
        {
            unsafe
            {
                SizeInBytes = sizeof(InputPositionNormal);
            }
        }

        public InputPositionNormal(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }

        ReadOnlyCollection<InputElement> IInputType.InputElements
        {
            get { return InputElements; }
        }

        #region Equatable

        public static bool operator ==(InputPositionNormal value1, InputPositionNormal value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(InputPositionNormal value1, InputPositionNormal value2)
        {
            return !value1.Equals(value2);
        }

        public bool Equals(InputPositionNormal other)
        {
            return Position == other.Position && Normal == other.Normal;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return Equals((InputPositionNormal) obj);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Normal.GetHashCode();
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return "[Position=" + Position + ", Normal=" + Normal + "]";
        }

        #endregion
    }
}
