#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public sealed class ModelMesh
    {
        public string Name { get; set; }

        public ModelBone ParentBone { get; set; }

        public ModelMeshPartCollection MeshParts { get; set; }

        public BoundingSphere BoundingSphere { get; set; }

        public ModelEffectCollection Effects { get; set; }
    }
}
