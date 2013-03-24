#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class Vector3BuilderBase : TypeBuilder
    {
        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Vector3"; }
        }

        public abstract void SetValues(float x, float y, float z);
    }
}
