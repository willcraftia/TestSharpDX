#region Using

using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

#endregion

namespace Libra.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InputPositionColorTexture : IInputType, IEquatable<InputPositionColorTexture>
    {
        public static readonly int SizeInBytes;

        public static ReadOnlyCollection<InputElement> InputElements = new ReadOnlyCollection<InputElement>(
            new[]
            {
                new InputElement("SV_Position", InputElementFormat.Vector3),
                new InputElement("COLOR",       InputElementFormat.Color),
                new InputElement("TEXCOORD",    InputElementFormat.Vector2),
            });

        public Vector3 Position;

        public Color Color;

        public Vector2 TexCoord;

        static InputPositionColorTexture()
        {
            unsafe
            {
                SizeInBytes = sizeof(InputPositionColorTexture);
            }
        }

        public InputPositionColorTexture(Vector3 position, Color color, Vector2 texCoord)
        {
            Position = position;
            Color = color;
            TexCoord = texCoord;
        }

        ReadOnlyCollection<InputElement> IInputType.InputElements
        {
            get { return InputElements; }
        }

        #region Equatable

        public static bool operator ==(InputPositionColorTexture value1, InputPositionColorTexture value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(InputPositionColorTexture value1, InputPositionColorTexture value2)
        {
            return !value1.Equals(value2);
        }

        public bool Equals(InputPositionColorTexture other)
        {
            return Position == other.Position && Color == other.Color && TexCoord == other.TexCoord;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return Equals((InputPositionColorTexture) obj);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Color.GetHashCode() ^ TexCoord.GetHashCode();
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return "[Position=" + Position + ", Color=" + Color + ", TexCoord=" + TexCoord + "]";
        }

        #endregion
    }
}
