#region Using

using System;

#endregion

namespace Felis.Samples.ReadXnb
{
    public struct BoundingSphere
    {
        public Vector3 Center;

        public float Radius;

        public BoundingSphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
    }
}
