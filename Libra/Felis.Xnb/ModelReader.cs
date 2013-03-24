#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public sealed class ModelReader : TypeReader
    {
        ModelBuilderBase builder;

        MatrixReader matrixReader;

        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.Model"; }
        }

        public override string ReaderName
        {
            get { return "Microsoft.Xna.Framework.Content.ModelReader"; }
        }

        protected internal override void Initialize(TypeReaderManager manager)
        {
            builder = manager.GetTypeBuilderByTargetType(TargetType) as ModelBuilderBase;

            matrixReader = new MatrixReader();
            matrixReader.Initialize(manager);

            base.Initialize(manager);
        }

        protected internal override object Read(ContentReader input)
        {
            builder.Begin();

            // Bone count
            var boneCount = input.ReadUInt32();
            builder.SetBoneCount(boneCount);

            builder.BeginBones();

            // Repeat <bone count>
            for (int i = 0; i < boneCount; i++)
            {
                builder.BeginBone(i);

                // Name
                builder.SetBoneName(input.ReadObject() as string);
                // Transform
                builder.SetBoneTransform(matrixReader.Read(input));

                builder.EndBone();
            }

            builder.EndBones();

            builder.BeginBoneHierarchies();

            // Repeat <bone count>: hierarchy
            for (int i = 0; i < boneCount; i++)
            {
                builder.BeginBoneHierarchy(i);

                // Parent bone
                builder.SetBoneHierarchyParentBone(ReadBoneReference(input, (int) boneCount));

                // Child bone count
                var childCount = input.ReadUInt32();
                builder.SetBoneHierarchyChildBoneCount(childCount);

                if (childCount != 0)
                {
                    builder.BeginBoneHierarchyChildBones();

                    for (int j = 0; j < childCount; j++)
                    {
                        builder.BeginBoneHierarchyChildBone(j);

                        // Child bone
                        builder.SetBoneHierarchyChildBone(ReadBoneReference(input, (int) boneCount));

                        builder.EndBoneHierarchyChildBone();
                    }

                    builder.EndBoneHierarchyChildBones();
                }

                builder.EndBoneHierarchy();
            }

            builder.EndBoneHierarchies();

            // Mesh count
            var meshCount = input.ReadUInt32();
            builder.SetMeshCount(meshCount);

            builder.BeginMeshes();

            // Repeat <mesh count>
            for (int i = 0; i < meshCount; i++)
            {
                builder.BeginMesh(i);

                // Name
                builder.SetMeshName(input.ReadObject() as string);

                // Parent bone
                builder.SetMeshParentBone(ReadBoneReference(input, (int) boneCount));

                // BoundingSphere
                builder.SetMeshBoundingSphere(
                    input.ReadSingle(), input.ReadSingle(), input.ReadSingle(),
                    input.ReadSingle());

                // Tag
                builder.SetMeshTag(input.ReadObject());

                // Mesh part count
                var partCount = input.ReadUInt32();
                builder.setMeshPartCount(partCount);

                builder.BeginMeshParts();

                // Repeat <mesh part count>
                for (int j = 0; j < partCount; j++)
                {
                    builder.BeginMeshPart(j);

                    // Vertex offset
                    builder.SetMeshPartVertexOffset(input.ReadUInt32());

                    // Num vertices
                    builder.SetMeshPartNumVertices(input.ReadUInt32());

                    // Start index
                    builder.SetMeshPartStartIndex(input.ReadUInt32());

                    // Primitive count
                    builder.SetMeshPartPrimitiveCount(input.ReadUInt32());

                    // Tag
                    builder.SetMeshPartTag(input.ReadObject());

                    // Vertex buffer
                    var vertexBufferId = input.ReadSharedResource(builder.GetMeshPartVertexBufferCallback());
                    builder.SetMeshPartVertexBuffer(vertexBufferId);

                    // Index buffer
                    var indexBufferId = input.ReadSharedResource(builder.GetMeshPartIndexBufferCallback());
                    builder.SetMeshPartIndexBuffer(indexBufferId);

                    // Effect
                    var effectId = input.ReadSharedResource(builder.GetMeshPartEffectCallback());
                    builder.SetMeshPartEffect(effectId);

                    builder.EndMeshPart();
                }

                builder.EndMeshParts();

                builder.EndMesh();
            }

            builder.EndMeshes();

            // Root bone
            builder.SetRootBone(ReadBoneReference(input, (int) boneCount));

            // Tag
            builder.SetTag(input.ReadObject());

            return builder.End();
        }

        int ReadBoneReference(ContentReader input, int boneCount)
        {
            if (boneCount < 255)
            {
                return input.ReadByte();
            }
            else
            {
                return (int) input.ReadUInt32();
            }
        }
    }
}
