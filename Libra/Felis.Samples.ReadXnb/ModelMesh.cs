#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class ModelMesh
    {
        public string Name;

        public ModelBone ParentBone;

        public BoundingSphere BoundingSphere;

        public object Tag;

        public ModelMeshPart[] MeshParts;

        public List<Effect> Effects = new List<Effect>();
    }
}
