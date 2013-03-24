#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class MatrixBuilder : MatrixBuilderBase
    {
        Matrix instance;

        public override Type ActualType
        {
            get { return typeof(Matrix); }
        }

        public override void SetValues(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44)
        {
            instance.M11 = m11;
            instance.M12 = m12;
            instance.M13 = m13;
            instance.M14 = m14;

            instance.M21 = m21;
            instance.M22 = m22;
            instance.M23 = m23;
            instance.M24 = m24;

            instance.M31 = m31;
            instance.M32 = m32;
            instance.M33 = m33;
            instance.M34 = m34;

            instance.M41 = m41;
            instance.M42 = m42;
            instance.M43 = m43;
            instance.M44 = m44;
        }

        public override void Begin()
        {
            instance = new Matrix();
        }

        public override object End()
        {
            return instance;
        }
    }
}
