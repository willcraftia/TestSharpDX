﻿#region Using

using System;
using System.Collections.Generic;

#endregion

// SharpDX.BoundingBox から移植。
// 一部インタフェースを XNA 形式へ変更。
// 一部ロジックを変更。

namespace Libra
{
    [Serializable]
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        public Vector3 Min;

        public Vector3 Max;

        public BoundingBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public Vector3[] GetCorners()
        {
            Vector3[] results = new Vector3[8];
            results[0] = new Vector3(Min.X, Max.Y, Max.Z);
            results[1] = new Vector3(Max.X, Max.Y, Max.Z);
            results[2] = new Vector3(Max.X, Min.Y, Max.Z);
            results[3] = new Vector3(Min.X, Min.Y, Max.Z);
            results[4] = new Vector3(Min.X, Max.Y, Min.Z);
            results[5] = new Vector3(Max.X, Max.Y, Min.Z);
            results[6] = new Vector3(Max.X, Min.Y, Min.Z);
            results[7] = new Vector3(Min.X, Min.Y, Min.Z);
            return results;
        }

        public bool Intersects(ref Ray ray)
        {
            float distance;
            return Collision.RayIntersectsBox(ref ray, ref this, out distance);
        }

        public bool Intersects(ref Ray ray, out float distance)
        {
            return Collision.RayIntersectsBox(ref ray, ref this, out distance);
        }

        public bool Intersects(ref Ray ray, out Vector3 point)
        {
            return Collision.RayIntersectsBox(ref ray, ref this, out point);
        }

        public PlaneIntersectionType Intersects(ref Plane plane)
        {
            return Collision.PlaneIntersectsBox(ref plane, ref this);
        }

        public bool Intersects(ref BoundingBox box)
        {
            return Collision.BoxIntersectsBox(ref this, ref box);
        }

        public bool Intersects(ref BoundingSphere sphere)
        {
            return Collision.BoxIntersectsSphere(ref this, ref sphere);
        }

        public ContainmentType Contains(ref Vector3 point)
        {
            return Collision.BoxContainsPoint(ref this, ref point);
        }

        public ContainmentType Contains(ref BoundingBox box)
        {
            return Collision.BoxContainsBox(ref this, ref box);
        }

        public ContainmentType Contains(ref BoundingSphere sphere)
        {
            return Collision.BoxContainsSphere(ref this, ref sphere);
        }

        public static void FromPoints(Vector3[] points, out BoundingBox result)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            for (int i = 0; i < points.Length; ++i)
            {
                Vector3.Min(ref min, ref points[i], out min);
                Vector3.Max(ref max, ref points[i], out max);
            }

            result = new BoundingBox(min, max);
        }

        public static BoundingBox FromPoints(Vector3[] points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            for (int i = 0; i < points.Length; ++i)
            {
                Vector3.Min(ref min, ref points[i], out min);
                Vector3.Max(ref max, ref points[i], out max);
            }

            return new BoundingBox(min, max);
        }

        public static void FromSphere(ref BoundingSphere sphere, out BoundingBox result)
        {
            result.Min = new Vector3(sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius);
            result.Max = new Vector3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius);
        }

        public static BoundingBox FromSphere(BoundingSphere sphere)
        {
            BoundingBox box;
            box.Min = new Vector3(sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius);
            box.Max = new Vector3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius);
            return box;
        }

        public static void Merge(ref BoundingBox value1, ref BoundingBox value2, out BoundingBox result)
        {
            Vector3.Min(ref value1.Min, ref value2.Min, out result.Min);
            Vector3.Max(ref value1.Max, ref value2.Max, out result.Max);
        }

        public static BoundingBox Merge(BoundingBox value1, BoundingBox value2)
        {
            BoundingBox box;
            Vector3.Min(ref value1.Min, ref value2.Min, out box.Min);
            Vector3.Max(ref value1.Max, ref value2.Max, out box.Max);
            return box;
        }

        #region IEquatable

        public static bool operator ==(BoundingBox left, BoundingBox right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BoundingBox left, BoundingBox right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return Min.GetHashCode() ^ Max.GetHashCode();
        }

        public bool Equals(BoundingBox other)
        {
            return Min == other.Min && Max == other.Max;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return Equals((Vector3) obj);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return "{Min:" + Min + " Max:" + Max + "}";
        }

        #endregion
    }
}
