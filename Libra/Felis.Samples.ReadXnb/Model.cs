#region Using

using System;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class Model
    {
        public ModelBone[] Bones;

        public ModelMesh[] Meshes;

        public ModelBone RootBone;

        public object Tag;
    }
}
