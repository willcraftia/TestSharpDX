#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class ModelBuilder : ModelBuilderBase
    {
        #region VertexBufferFixup

        struct VertexBufferFixup
        {
            ModelMeshPart part;

            public VertexBufferFixup(ModelMeshPart part)
            {
                this.part = part;
            }

            public void Fixup(object vertexBuffer)
            {
                part.VertexBuffer = vertexBuffer as VertexBuffer;
            }
        }

        #endregion

        #region IndexBufferFixup

        struct IndexBufferFixup
        {
            ModelMeshPart part;

            public IndexBufferFixup(ModelMeshPart part)
            {
                this.part = part;
            }

            public void Fixup(object indexBuffer)
            {
                part.IndexBuffer = indexBuffer as IndexBuffer;
            }
        }

        #endregion

        #region EffectFixup

        struct EffectFixup
        {
            ModelMesh mesh;

            ModelMeshPart part;

            public EffectFixup(ModelMesh mesh, ModelMeshPart part)
            {
                this.mesh = mesh;
                this.part = part;
            }

            public void Fixup(object effect)
            {
                part.Effect = effect as Effect;
                mesh.Effects.Add(part.Effect);
            }
        }

        #endregion

        Model instance;

        ModelBone currentBone;

        int currentChildBoneIndex;

        ModelMesh currentMesh;

        ModelMeshPart currentMeshPart;

        public override Type ActualType
        {
            get { return typeof(Model); }
        }

        public override void SetBoneCount(uint value)
        {
            instance.Bones = new ModelBone[value];
        }

        public override void BeginBone(int index)
        {
            instance.Bones[index] = new ModelBone();
            currentBone = instance.Bones[index];
        }

        public override void SetBoneName(string value)
        {
            currentBone.Name = value;
        }

        public override void SetBoneTransform(object value)
        {
            currentBone.Transform = (Matrix) value;
        }

        public override void BeginBoneHierarchy(int index)
        {
            currentBone = instance.Bones[index];
        }

        public override void SetBoneHierarchyParentBone(int value)
        {
            if (value != 0)
            {
                currentBone.Parent = instance.Bones[value - 1];
            }
        }

        public override void SetBoneHierarchyChildBoneCount(uint value)
        {
            currentBone.Children = new ModelBone[value];
        }

        public override void BeginBoneHierarchyChildBone(int index)
        {
            currentChildBoneIndex = index;
        }

        public override void SetBoneHierarchyChildBone(int value)
        {
            if (value != 0)
            {
                currentBone.Children[currentChildBoneIndex] = instance.Bones[value - 1];
            }
        }

        public override void SetMeshCount(uint value)
        {
            instance.Meshes = new ModelMesh[value];
        }
    
        public override void BeginMesh(int index)
        {
            instance.Meshes[index] = new ModelMesh();
            currentMesh = instance.Meshes[index];
        }

        public override void SetMeshName(string value)
        {
            currentMesh.Name = value;
        }

        public override void SetMeshParentBone(int value)
        {
            if (value != 0)
            {
                currentMesh.ParentBone = instance.Bones[value - 1];
            }
        }

        public override void SetMeshBoundingSphere(float x, float y, float z, float radius)
        {
            currentMesh.BoundingSphere = new BoundingSphere(new Vector3(x, y, z), radius);
        }

        public override void SetMeshTag(object value)
        {
            currentMesh.Tag = value;
        }

        public override void setMeshPartCount(uint value)
        {
            currentMesh.MeshParts = new ModelMeshPart[value];
        }

        public override void BeginMeshPart(int index)
        {
            currentMesh.MeshParts[index] = new ModelMeshPart();
            currentMeshPart = currentMesh.MeshParts[index];
        }

        public override void SetMeshPartVertexOffset(uint value)
        {
            currentMeshPart.VertexOffset = (int) value;
        }

        public override void SetMeshPartNumVertices(uint value)
        {
            currentMeshPart.NumVertices = (int) value;
        }

        public override void SetMeshPartStartIndex(uint value)
        {
            currentMeshPart.StartIndex = (int) value;
        }

        public override void SetMeshPartPrimitiveCount(uint value)
        {
            currentMeshPart.PrimitiveCount = (int) value;
        }

        public override void SetMeshPartTag(object value)
        {
            currentMeshPart.Tag = value;
        }

        public override Action<object> GetMeshPartVertexBufferCallback()
        {
            return new VertexBufferFixup(currentMeshPart).Fixup;
        }

        public override Action<object> GetMeshPartIndexBufferCallback()
        {
            return new IndexBufferFixup(currentMeshPart).Fixup;
        }

        public override Action<object> GetMeshPartEffectCallback()
        {
            return new EffectFixup(currentMesh, currentMeshPart).Fixup;
        }

        public override void SetRootBone(int value)
        {
            if (value != 0)
            {
                instance.RootBone = instance.Bones[value - 1];
            }
        }

        public override void SetTag(object value)
        {
            instance.Tag = value;
        }

        public override void Begin()
        {
            instance = new Model();
        }

        public override object End()
        {
            return instance;
        }
    }
}
