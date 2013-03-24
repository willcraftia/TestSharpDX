#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class ModelBuilderBase : TypeBuilder
    {
        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.Model"; }
        }

        public abstract void SetBoneCount(uint value);

        public virtual void BeginBones() { }

        public abstract void BeginBone(int index);

        public abstract void SetBoneName(string value);

        public abstract void SetBoneTransform(object value);

        public virtual void EndBone() { }

        public virtual void EndBones() { }

        public virtual void BeginBoneHierarchies() { }

        public abstract void BeginBoneHierarchy(int index);

        public abstract void SetBoneHierarchyParentBone(int value);

        public abstract void SetBoneHierarchyChildBoneCount(uint value);

        public virtual void BeginBoneHierarchyChildBones() { }

        public abstract void BeginBoneHierarchyChildBone(int index);

        public abstract void SetBoneHierarchyChildBone(int value);

        public virtual void EndBoneHierarchyChildBone() { }

        public virtual void EndBoneHierarchyChildBones() { }

        public virtual void EndBoneHierarchy() { }

        public virtual void EndBoneHierarchies() { }

        public abstract void SetMeshCount(uint value);

        public virtual void BeginMeshes() { }

        public abstract void BeginMesh(int index);

        public abstract void SetMeshName(string value);

        public abstract void SetMeshParentBone(int value);

        public abstract void SetMeshBoundingSphere(float x, float y, float z, float radius);

        public abstract void SetMeshTag(object value);

        public abstract void setMeshPartCount(uint value);

        public virtual void BeginMeshParts() { }

        public abstract void BeginMeshPart(int index);

        public abstract void SetMeshPartVertexOffset(uint value);

        public abstract void SetMeshPartNumVertices(uint value);

        public abstract void SetMeshPartStartIndex(uint value);

        public abstract void SetMeshPartPrimitiveCount(uint value);

        public abstract void SetMeshPartTag(object value);

        public abstract Action<object> GetMeshPartVertexBufferCallback();

        public virtual void SetMeshPartVertexBuffer(int value) { }

        public abstract Action<object> GetMeshPartIndexBufferCallback();

        public virtual void SetMeshPartIndexBuffer(int value) { }

        public abstract Action<object> GetMeshPartEffectCallback();

        public virtual void SetMeshPartEffect(int value) { }

        public virtual void EndMeshPart() { }

        public virtual void EndMeshParts() { }

        public abstract void SetRootBone(int value);

        public abstract void SetTag(object value);

        public virtual void EndMesh() { }

        public virtual void EndMeshes() { }
    }
}
