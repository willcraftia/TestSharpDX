#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Graphics
{
    public sealed class ModelMesh
    {
        IDevice device;

        public string Name { get; private set; }

        public ModelBone ParentBone { get; private set; }

        public ModelMeshPartCollection MeshParts { get; private set; }

        public ModelMesh(IDevice device, string name, ModelBone parentBone, IList<ModelMeshPart> meshParts)
        {
            if (device == null) throw new ArgumentNullException("device");
            if (name == null) throw new ArgumentNullException("name");
            if (parentBone == null) throw new ArgumentNullException("parentBone");
            if (meshParts == null) throw new ArgumentNullException("meshParts");

            this.device = device;
            Name = name;
            ParentBone = parentBone;
            MeshParts = new ModelMeshPartCollection(meshParts);
        }
    }
}
