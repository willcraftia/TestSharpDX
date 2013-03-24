#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class Vector3Builder : Vector3BuilderBase
    {
        Vector3 instance;

        public override Type ActualType
        {
            get { return typeof(Vector3); }
        }

        public override void SetValues(float x, float y, float z)
        {
            instance.X = x;
            instance.Y = y;
            instance.Z = z;
        }

        public override void Begin()
        {
            instance = new Vector3();
        }

        public override object End()
        {
            return instance;
        }
    }
}
