#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class ModelBuilder : ModelBuilderBase<Model>
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

        protected override void SetBoneCount(uint value)
        {
            instance.Bones = new ModelBone[value];
        }

        protected override void BeginBone(int index)
        {
            instance.Bones[index] = new ModelBone();
            currentBone = instance.Bones[index];
        }

        protected override void SetBoneName(string value)
        {
            currentBone.Name = value;
        }

        protected override void SetBoneTransform(object value)
        {
            currentBone.Transform = (Matrix) value;
        }

        protected override void BeginBoneHierarchy(int index)
        {
            currentBone = instance.Bones[index];
        }

        protected override void SetBoneHierarchyParentBone(int value)
        {
            if (value != 0)
            {
                currentBone.Parent = instance.Bones[value - 1];
            }
        }

        protected override void SetBoneHierarchyChildBoneCount(uint value)
        {
            currentBone.Children = new ModelBone[value];
        }

        protected override void BeginBoneHierarchyChildBone(int index)
        {
            currentChildBoneIndex = index;
        }

        protected override void SetBoneHierarchyChildBone(int value)
        {
            if (value != 0)
            {
                currentBone.Children[currentChildBoneIndex] = instance.Bones[value - 1];
            }
        }

        protected override void SetMeshCount(uint value)
        {
            instance.Meshes = new ModelMesh[value];
        }
    
        protected override void BeginMesh(int index)
        {
            instance.Meshes[index] = new ModelMesh();
            currentMesh = instance.Meshes[index];
        }

        protected override void SetMeshName(string value)
        {
            currentMesh.Name = value;
        }

        protected override void SetMeshParentBone(int value)
        {
            if (value != 0)
            {
                currentMesh.ParentBone = instance.Bones[value - 1];
            }
        }

        protected override void SetMeshBoundingSphere(object value)
        {
            currentMesh.BoundingSphere = (BoundingSphere) value;
        }

        protected override void SetMeshTag(object value)
        {
            currentMesh.Tag = value;
        }

        protected override void SetMeshPartCount(uint value)
        {
            currentMesh.MeshParts = new ModelMeshPart[value];
        }

        protected override void BeginMeshPart(int index)
        {
            currentMesh.MeshParts[index] = new ModelMeshPart();
            currentMeshPart = currentMesh.MeshParts[index];
        }

        protected override void SetMeshPartVertexOffset(uint value)
        {
            currentMeshPart.VertexOffset = (int) value;
        }

        protected override void SetMeshPartNumVertices(uint value)
        {
            currentMeshPart.NumVertices = (int) value;
        }

        protected override void SetMeshPartStartIndex(uint value)
        {
            currentMeshPart.StartIndex = (int) value;
        }

        protected override void SetMeshPartPrimitiveCount(uint value)
        {
            currentMeshPart.PrimitiveCount = (int) value;
        }

        protected override void SetMeshPartTag(object value)
        {
            currentMeshPart.Tag = value;
        }

        protected override Action<object> GetMeshPartVertexBufferCallback()
        {
            return new VertexBufferFixup(currentMeshPart).Fixup;
        }

        protected override Action<object> GetMeshPartIndexBufferCallback()
        {
            return new IndexBufferFixup(currentMeshPart).Fixup;
        }

        protected override Action<object> GetMeshPartEffectCallback()
        {
            return new EffectFixup(currentMesh, currentMeshPart).Fixup;
        }

        protected override void SetRootBone(int value)
        {
            if (value != 0)
            {
                instance.RootBone = instance.Bones[value - 1];
            }
        }

        protected override void SetTag(object value)
        {
            instance.Tag = value;
        }

        protected override void Begin(object deviceContext)
        {
            instance = new Model();
        }

        protected override object End()
        {
            return instance;
        }
    }
}
