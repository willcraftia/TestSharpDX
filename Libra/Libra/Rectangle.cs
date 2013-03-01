#region Using

using System;
using System.Runtime.InteropServices;

#endregion

namespace Libra
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle : IEquatable<Rectangle>
    {
        public static readonly Rectangle Empty = new Rectangle();

        public int Left;
        
        public int Top;
        
        public int Right;
        
        public int Bottom;

        public int X
        {
            get { return Left; }
        }

        public int Y
        {
            get { return Top; }
        }

        public int Width
        {
            get { return Right - Left; }
        }

        public int Height
        {
            get { return Bottom - Top; }
        }

        public Rectangle(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public bool Contains(int x, int y)
        {
            if (x >= Left && x <= Right && y >= Top && y <= Bottom)
            {
                return true;
            }
            return false;
        }

        public bool Contains(float x, float y)
        {
            if (x >= Left && x <= Right && y >= Top && y <= Bottom)
            {
                return true;
            }
            return false;
        }

        public bool Contains(Vector2 vector2D)
        {
            if (vector2D.X >= Left && vector2D.X <= Right && vector2D.Y >= Top && vector2D.Y <= Bottom)
            {
                return true;
            }
            return false;
        }

        public bool Contains(Point point)
        {
            if (point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom)
            {
                return true;
            }
            return false;
        }

        #region IEquatable

        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Rectangle other)
        {
            return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return Equals((Rectangle) obj);
        }

        public override int GetHashCode()
        {
            return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return "{Left:" + Left + " Top:" + Top + " Right:" + Right + " Bottom:" + Bottom + "}";
        }

        #endregion
    }
}
