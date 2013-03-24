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

        protected ModelBuilderBase() { }

        protected internal abstract void SetBoneCount(uint value);

        protected internal virtual void BeginBones() { }

        protected internal abstract void BeginBone(int index);

        protected internal abstract void SetBoneName(string value);

        protected internal abstract void SetBoneTransform(object value);

        protected internal virtual void EndBone() { }

        protected internal virtual void EndBones() { }

        protected internal virtual void BeginBoneHierarchies() { }

        protected internal abstract void BeginBoneHierarchy(int index);

        protected internal abstract void SetBoneHierarchyParentBone(int value);

        protected internal abstract void SetBoneHierarchyChildBoneCount(uint value);

        protected internal virtual void BeginBoneHierarchyChildBones() { }

        protected internal abstract void BeginBoneHierarchyChildBone(int index);

        protected internal abstract void SetBoneHierarchyChildBone(int value);

        protected internal virtual void EndBoneHierarchyChildBone() { }

        protected internal virtual void EndBoneHierarchyChildBones() { }

        protected internal virtual void EndBoneHierarchy() { }

        protected internal virtual void EndBoneHierarchies() { }

        protected internal abstract void SetMeshCount(uint value);

        protected internal virtual void BeginMeshes() { }

        protected internal abstract void BeginMesh(int index);

        protected internal abstract void SetMeshName(string value);

        protected internal abstract void SetMeshParentBone(int value);

        protected internal abstract void SetMeshBoundingSphere(object value);

        protected internal abstract void SetMeshTag(object value);

        protected internal abstract void SetMeshPartCount(uint value);

        protected internal virtual void BeginMeshParts() { }

        protected internal abstract void BeginMeshPart(int index);

        protected internal abstract void SetMeshPartVertexOffset(uint value);

        protected internal abstract void SetMeshPartNumVertices(uint value);

        protected internal abstract void SetMeshPartStartIndex(uint value);

        protected internal abstract void SetMeshPartPrimitiveCount(uint value);

        protected internal abstract void SetMeshPartTag(object value);

        protected internal abstract Action<object> GetMeshPartVertexBufferCallback();

        protected internal virtual void SetMeshPartVertexBuffer(int value) { }

        protected internal abstract Action<object> GetMeshPartIndexBufferCallback();

        protected internal virtual void SetMeshPartIndexBuffer(int value) { }

        protected internal abstract Action<object> GetMeshPartEffectCallback();

        protected internal virtual void SetMeshPartEffect(int value) { }

        protected internal virtual void EndMeshPart() { }

        protected internal virtual void EndMeshParts() { }

        protected internal abstract void SetRootBone(int value);

        protected internal abstract void SetTag(object value);

        protected internal virtual void EndMesh() { }

        protected internal virtual void EndMeshes() { }
    }

    public abstract class ModelBuilderBase<T> : ModelBuilderBase
    {
        public override Type ActualType
        {
            get { return typeof(T); }
        }

        protected ModelBuilderBase() { }
    }
}
