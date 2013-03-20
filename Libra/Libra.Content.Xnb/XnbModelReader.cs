#region Using

using System;
using System.Collections.Generic;
using Libra.Graphics;

#endregion

namespace Libra.Content.Xnb
{
    [XnbTypeReaderAttribute("Microsoft.Xna.Framework.Content.ModelReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
    public sealed class XnbModelReader : XnbTypeReader<Model>
    {
        #region VertexBufferFixup

        struct VertexBufferFixup
        {
            ModelMeshPart part;

            public VertexBufferFixup(ModelMeshPart part)
            {
                this.part = part;
            }

            public void Fixup(VertexBuffer vertexBuffer)
            {
                part.VertexBuffer = vertexBuffer;
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

            public void Fixup(IndexBuffer indexBuffer)
            {
                part.IndexBuffer = indexBuffer;
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

            public void Fixup(IEffect effect)
            {
                part.Effect = effect;

                if (mesh.Effects == null)
                    mesh.Effects = new ModelEffectCollection();

                mesh.Effects.Add(effect);
            }
        }

        #endregion

        protected internal override Model Read(XnbReader input, Model existingInstance)
        {
            var boneCount = (int) input.ReadUInt32();

            var bones = new List<ModelBone>(boneCount);

            // 全ボーン。
            for (int i = 0; i < boneCount; i++)
            {
                var bone = new ModelBone();
                bone.Index = i;

                // Name
                bone.Name = input.ReadObject<string>();
                // Transform
                bone.Transform = input.ReadMatrix();

                bones.Add(bone);
            }

            // ボーン階層構造。
            for (int i = 0; i < boneCount; i++)
            {
                var bone = bones[i];

                var parentReference = ReadBoneReference(input, boneCount);

                ModelBone parentBone;
                if (parentReference == 0)
                {
                    parentBone = null;
                }
                else
                {
                    parentBone = bones[parentReference - 1];
                }

                var childCount = (int) input.ReadUInt32();

                var children = new List<ModelBone>(childCount);

                if (childCount != 0)
                {
                    for (int j = 0; j < childCount; j++)
                    {
                        int childReference = ReadBoneReference(input, boneCount);

                        var child = bones[childReference - 1];
                        children.Add(child);
                    }
                }

                bone.Parent = parentBone;
                bone.Children = new ModelBoneCollection(children);
            }

            // 全メッシュ。
            var meshCount = (int) input.ReadUInt32();

            var meshes = new List<ModelMesh>(meshCount);

            for (int i = 0; i < meshCount; i++)
            {
                var mesh = new ModelMesh();

                // Name
                mesh.Name = input.ReadObject<string>();

                // Parent bone
                var boneReference = ReadBoneReference(input, boneCount);
                if (boneReference != 0)
                {
                    mesh.ParentBone = bones[boneReference - 1];
                }

                // BoundingSphere
                mesh.BoundingSphere = input.ReadBoundingSphere();

                // Tag
                // 読み込むのみで使用しない。
                input.ReadObject<object>();

                // Mesh part count
                var partCount = (int) input.ReadUInt32();

                var parts = new List<ModelMeshPart>(partCount);

                for (int j = 0; j < partCount; j++)
                {
                    var part = new ModelMeshPart();

                    // Vertex offset
                    part.VertexOffset = (int) input.ReadUInt32();

                    // Num vertices
                    part.NumVertices = (int) input.ReadUInt32();

                    // Start index
                    part.StartIndex = (int) input.ReadUInt32();

                    // Primitive count
                    part.PrimitiveCount = (int) input.ReadUInt32();

                    // Tag
                    // 読み込むのみで使用しない。
                    input.ReadObject<object>();

                    // Vertex buffer
                    input.ReadSharedResource<VertexBuffer>(new VertexBufferFixup(part).Fixup);

                    // Index buffer
                    input.ReadSharedResource<IndexBuffer>(new IndexBufferFixup(part).Fixup);

                    // Effect
                    input.ReadSharedResource<IEffect>(new EffectFixup(mesh, part).Fixup);

                    parts.Add(part);
                }

                mesh.MeshParts = new ModelMeshPartCollection(parts);

                meshes.Add(mesh);
            }

            // Root bone
            int rootReference = ReadBoneReference(input, boneCount);
            var rootBone = bones[rootReference - 1];

            // Tag
            // 読み込むのみで使用しない。
            input.ReadObject<object>();

            var model = new Model();
            model.Root = rootBone;
            model.Bones = new ModelBoneCollection(bones);
            model.Meshes = new ModelMeshCollection(meshes);

            return model;
        }

        int ReadBoneReference(XnbReader input, int boneCount)
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
