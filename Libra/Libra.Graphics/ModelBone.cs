#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Graphics
{
    public sealed class ModelBone
    {
        public Matrix Transform;

        public string Name { get; private set; }

        public int Index { get; internal set; }

        public ModelBone Parent { get; private set; }

        public ModelBoneCollection Children { get; private set; }

        public ModelBone(string name, IList<ModelBone> children)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (children == null) throw new ArgumentNullException("children");

            Name = name;
            Children = new ModelBoneCollection(children);

            for (int i = 0; i < children.Count; i++)
            {
                children[i].Parent = this;
            }
        }
    }
}
