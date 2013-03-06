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
        static ReadOnlyCollection<InputElement> inputElements = new ReadOnlyCollection<InputElement>(
            new[]
            {
                new InputElement("SV_Position", 0, InputElementFormat.Vector3, 0,  0),
                new InputElement("NORMAL",      0, InputElementFormat.Vector3, 0, 12),
            });

        public Vector3 Position;

        public Vector3 Normal;

        public InputPositionNormal(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }

        public ReadOnlyCollection<InputElement> InputElements
        {
            get { return inputElements; }
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
