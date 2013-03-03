﻿#region Using

using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

#endregion

namespace Libra.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct InputPositionColor : IInputType, IEquatable<InputPositionColor>
    {
        static ReadOnlyCollection<InputElement> inputElements = new ReadOnlyCollection<InputElement>(
            new[]
            {
                new InputElement("POSITION", 0, InputElementFormat.Vector3, 0,  0),
                new InputElement("COLOR",    0, InputElementFormat.Color,   0, 12),
            });

        public Vector3 Position;

        public Color Color;

        public InputPositionColor(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
        }

        public ReadOnlyCollection<InputElement> InputElements
        {
            get { return inputElements; }
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
