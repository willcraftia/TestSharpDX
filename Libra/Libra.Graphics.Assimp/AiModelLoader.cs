#region Using

using System;
using System.Collections.Generic;
using Assimp;
using Assimp.Configs;

#endregion

namespace Libra.Graphics.Assimp
{
    public sealed class AiModelLoader
    {
        IDevice device;

        // TODO
        // これらのプロパティは Load メソッドの引数とすべきか？

        public bool UseColor4 { get; set; }

        public IndexFormat IndexFormat { get; set; }

        public AiModelLoader(IDevice device)
        {
            if (device == null) throw new ArgumentNullException("device");

            this.device = device;

            UseColor4 = false;
            IndexFormat = IndexFormat.SixteenBits;
        }

        public Model Load(string path)
        {
            var importer = new AssimpImporter();

            // aiPostProcessSteps
            // http://assimp.sourceforge.net/lib_html/postprocess_8h.html
            //
            // JoinIdenticalVertices
            //      頂点が複数の面で共有されるように調整される。
            //      インデックス バッファを用いるなら必須とのこと。
            //
            // Triangulate
            //      三角形化。
            //
            // FlipUVs
            //      左手座標系のための変換に利用できるとのこと。ここでは不要。
            //
            // SortByPrimitiveType
            //      通常の描画では点や線は不要であるため、
            //      ソート後にそれらを除去するために利用できるとのこと。
            //
            // FindInvalidData
            //      不正データを検出 (ゼロ法線ベクトルなど) し、
            //      それらを除去する、あるいは、修正する。

            // 点と線を除去。
            importer.SetConfig(new SortByPrimitiveTypeConfig(PrimitiveType.Point | PrimitiveType.Line));

            var scene = importer.ImportFile(path,
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


        void ParseScene(Scene scene, Node node, ref Matrix baseTransform, Node targetNode)
        {
            Matrix transform;
            ToMatrix(node.Transform, out transform);

            if (node.MeshCount == 0)
            {
                // メッシュ無しで子ノードも無いならば、これ以上の探索は不要。
                if (!node.HasChildren)
                    return;

                Matrix newBaseTransform;
                Matrix.Multiply(ref baseTransform, ref transform, out newBaseTransform);

                foreach (var child in node.Children)
                    ParseScene(scene, child, ref newBaseTransform, targetNode);
            }
            else
            {
                if (targetNode == null)
                {
                    // モデルの基点となるノードを検出。

                    // ここまでの変換行列をルート ボーンの変換行列に使用。

                }
                else
                {
                }
            }

        }

        void ParseModel(Scene scene, Node node, ref Matrix baseTransform)
        {
            foreach (var meshIndex in node.MeshIndices)
            {
                var mesh = scene.Meshes[meshIndex];

                ParseMesh(scene, node, mesh, ref baseTransform);
            }

        }

        void ParseMesh(Scene scene, Node node, Mesh mesh, ref Matrix baseTransform)
        {
            
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

        ModelMesh ParseMesh(Mesh mesh, IList<ModelBone> bones)
        {
            var vertexBuffer = CreateVertexBuffer(mesh, device);
            var indexBuffer = CreateIndexBuffer(mesh, device);

            var meshPart = new ModelMeshPart(vertexBuffer, indexBuffer);

            var meshParts = new List<ModelMeshPart>(1);
            meshParts.Add(meshPart);

            throw new NotImplementedException();
        }

        VertexBuffer CreateVertexBuffer(Mesh mesh, IDevice device)
        {
            var vertexStrideInSingles = CalculateVertexStrideInSingles(mesh);
            var vertexCount = mesh.VertexCount;

            var vertices = new float[vertexStrideInSingles * vertexCount];

            Color4D[][] colors = null;
            if (0 < mesh.VertexColorChannelCount)
            {
                colors = new Color4D[mesh.VertexColorChannelCount][];
                for (int i = 0; i < mesh.VertexColorChannelCount; i++)
                {
                    colors[i] = mesh.GetVertexColors(i);
                }
            }

            Vector3D[][] texCoords = null;
            if (0 < mesh.TextureCoordsChannelCount)
            {
                texCoords = new Vector3D[mesh.TextureCoordsChannelCount][];
                for (int i = 0; i < mesh.TextureCoordsChannelCount; i++)
                {
                    texCoords[i] = mesh.GetTextureCoords(i);
                }
            }

            int e = 0;

            for (int i = 0; i < vertexCount; i++)
            {
                // 頂点位置。
                vertices[e++] = mesh.Vertices[i].X;
                vertices[e++] = mesh.Vertices[i].Y;
                vertices[e++] = mesh.Vertices[i].Z;

                // 法線。
                if (mesh.HasNormals)
                {
                    vertices[e++] = mesh.Normals[i].X;
                    vertices[e++] = mesh.Normals[i].Y;
                    vertices[e++] = mesh.Normals[i].Z;
                }

                // 頂点カラー。
                if (0 < mesh.VertexColorChannelCount)
                {
                    for (int c = 0; c < mesh.VertexColorChannelCount; c++)
                    {
                        var color = colors[c][i];

                        vertices[e++] = color.R;
                        vertices[e++] = color.G;
                        vertices[e++] = color.B;

                        if (UseColor4)
                            vertices[e++] = color.A;
                    }
                }

                // テクスチャ座標。
                if (0 < mesh.TextureCoordsChannelCount)
                {
                    if (0 < mesh.TextureCoordsChannelCount)
                    {
                        for (int t = 0; t < mesh.TextureCoordsChannelCount; t++)
                        {
                            var texCoord = texCoords[t][i];

                            var componentCount = mesh.GetUVComponentCount(t);

                            vertices[e++] = texCoord.X;
                            if (1 < componentCount) vertices[e++] = texCoord.Y;
                            if (2 < componentCount) vertices[e++] = texCoord.Z;
                        }
                    }
                }

                // TODO
                //
                // Tangents と BiTangents。
                // 当面は無視。
            }

            var vertexStride = vertexStrideInSingles * 4;

            var vertexBuffer = device.CreateVertexBuffer();
            vertexBuffer.Initialize(vertices, vertexStride);

            return vertexBuffer;
        }

        int CalculateVertexStrideInSingles(Mesh mesh)
        {
            // 頂点位置。
            int stride = 3;

            // 法線。
            if (mesh.HasNormals)
            {
                stride += 3;
            }

            // 頂点カラー。
            if (0 < mesh.VertexColorChannelCount)
            {
                var colorElementCount = (UseColor4) ? 4 : 3;
                stride += mesh.VertexColorChannelCount * colorElementCount;
            }

            // テクスチャ座標。
            if (0 < mesh.TextureCoordsChannelCount)
            {
                for (int i = 0; i < mesh.TextureCoordsChannelCount; i++)
                {
                    stride += mesh.GetUVComponentCount(i);
                }
            }

            // TODO
            //
            // Tangents と BiTangents。
            // 当面は無視。

            return stride;
        }

        IndexBuffer CreateIndexBuffer(Mesh mesh, IDevice device)
        {
            var indexBuffer = device.CreateIndexBuffer();
            indexBuffer.Format = IndexFormat;

            if (IndexFormat == IndexFormat.SixteenBits)
            {
                indexBuffer.Initialize(mesh.GetShortIndices());
            }
            else
            {
                indexBuffer.Initialize(mesh.GetIndices());
            }

            return indexBuffer;
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
