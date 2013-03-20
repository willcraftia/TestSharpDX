﻿#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public sealed class ModelMesh
    {
        public string Name { get; set; }

        public ModelBone ParentBone { get; set; }

        public ModelMeshPartCollection MeshParts { get; set; }

        public BoundingSphere BoundingSphere { get; set; }

        public ModelEffectCollection Effects { get; set; }

        public void Draw(DeviceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            for (int i = 0; i < MeshParts.Count; i++)
            {
                var part = MeshParts[i];

                if (part.PrimitiveCount != 0)
                {
                    context.InputAssemblerStage.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    context.InputAssemblerStage.SetVertexBuffer(0, part.VertexBuffer, part.VertexOffset);
                    context.InputAssemblerStage.IndexBuffer = part.IndexBuffer;

                    part.Effect.Apply(context);

                    // TODO
                    // ここの baseVertexLocation と先の offset の違いはなにか？
                    context.DrawIndexed(part.PrimitiveCount * 3, part.StartIndex);
                }
            }
        }
    }
}
