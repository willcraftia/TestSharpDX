#region Using

using System;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class ModelBone
    {
        public string Name;

        public Matrix Transform;

        public ModelBone Parent;

        public ModelBone[] Children;
    }
}
