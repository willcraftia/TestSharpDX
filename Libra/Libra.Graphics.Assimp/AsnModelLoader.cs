#region Using

using System;
using System.Collections.Generic;
using Assimp;

#endregion

namespace Libra.Graphics.Assimp
{
    public sealed class AsnModelLoader
    {
        public Model Load(IDevice device, string path)
        {
            if (device == null) throw new ArgumentNullException("device");

            var importer = new AssimpImporter();

            var scene = importer.ImportFile(path,
                PostProcessSteps.FlipUVs |
                PostProcessSteps.JoinIdenticalVertices |
                PostProcessSteps.Triangulate |
                PostProcessSteps.SortByPrimitiveType |
                PostProcessSteps.FindInvalidData);

            int boneCount = 1;
            CountNode(scene.RootNode, ref boneCount);

            var bones = new List<ModelBone>(boneCount);

            var rootBone = ParseNode(scene.RootNode, bones);

            throw new NotImplementedException();
        }

        void CountNode(Node node, ref int result)
        {
            if (node.HasChildren)
            {
                result += node.ChildCount;

                foreach (var child in node.Children)
                {
                    CountNode(child, ref result);
                }
            }
        }

        ModelBone ParseNode(Node node, IList<ModelBone> bones)
        {
            List<ModelBone> children;

            if (node.HasChildren)
            {
                children = new List<ModelBone>(node.ChildCount);
                for (int i = 0; i < node.ChildCount; i++)
                {
                    var child = ParseNode(node.Children[i], bones);
                    children.Add(child);
                }
            }
            else
            {
                children = new List<ModelBone>(0);
            }

            var bone = new ModelBone(node.Name, children);
            ToMatrix(node.Transform, out bone.Transform);

            bones.Add(bone);

            return bone;
        }

        ModelMesh ParseMesh(Mesh mesh)
        {
            // TODO
            //
            // 順序を決めて頂点バッファへ入れる。


            throw new NotImplementedException();
        }

        void ToMatrix(Matrix4x4 value, out Matrix result)
        {
            result = new Matrix
            {
                M11 = value.A1,
                M12 = value.A1,
                M13 = value.A2,
                M14 = value.A3,
                M21 = value.B1,
                M22 = value.B1,
                M23 = value.B2,
                M24 = value.B3,
                M31 = value.C1,
                M32 = value.C1,
                M33 = value.C2,
                M34 = value.C3,
                M41 = value.D1,
                M42 = value.D1,
                M43 = value.D2,
                M44 = value.D3
            };
        }
    }
}
