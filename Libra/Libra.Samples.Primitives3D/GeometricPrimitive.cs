﻿#region Using

using System;
using System.Collections.Generic;
using Libra.Graphics;

#endregion

namespace Libra.Samples.Primitives3D
{
    public abstract class GeometricPrimitive : IDisposable
    {
        List<InputPositionNormal> vertices = new List<InputPositionNormal>();

        List<ushort> indices = new List<ushort>();

        VertexBuffer vertexBuffer;
        
        IndexBuffer indexBuffer;
        
        BasicEffect basicEffect;

        protected IDevice Device { get; private set; }

        protected GeometricPrimitive(IDevice device)
        {
            if (device == null) throw new ArgumentNullException("device");

            Device = device;
        }

        protected void AddVertex(Vector3 position, Vector3 normal)
        {
            vertices.Add(new InputPositionNormal(position, normal));
        }

        protected void AddIndex(int index)
        {
            if (index > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("index");

            indices.Add((ushort) index);
        }

        protected int CurrentVertex
        {
            get { return vertices.Count; }
        }

        protected void InitializePrimitive()
        {
            vertexBuffer = Device.CreateVertexBuffer();
            vertexBuffer.Usage = ResourceUsage.Immutable;
            vertexBuffer.Initialize(vertices.ToArray());

            indexBuffer = Device.CreateIndexBuffer();
            indexBuffer.Format = IndexFormat.SixteenBits;
            indexBuffer.Usage = ResourceUsage.Immutable;
            indexBuffer.Initialize(indices.ToArray());

            basicEffect = new BasicEffect(Device);
            basicEffect.EnableDefaultLighting();
        }

        public void Draw(DeviceContext context, IEffect effect)
        {
            context.InputAssemblerStage.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssemblerStage.SetVertexBuffer<InputPositionNormal>(0, vertexBuffer);
            context.InputAssemblerStage.IndexBuffer = indexBuffer;

            effect.Apply(context);

            context.DrawIndexed(indices.Count);
        }

        public void Draw(DeviceContext context, Matrix world, Matrix view, Matrix projection, Color color)
        {
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.DiffuseColor = color.ToVector3();
            basicEffect.Alpha = color.A / 255.0f;

            context.OutputMergerStage.DepthStencilState = DepthStencilState.Default;

            if (color.A < 255)
            {
                context.OutputMergerStage.BlendState = BlendState.AlphaBlend;
            }
            else
            {
                context.OutputMergerStage.BlendState = BlendState.Opaque;
            }

            Draw(context, basicEffect);
        }

        #region IDisposable

        bool disposed;

        ~GeometricPrimitive()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                if (vertexBuffer != null)
                    vertexBuffer.Dispose();

                if (indexBuffer != null)
                    indexBuffer.Dispose();

                if (basicEffect != null)
                    basicEffect.Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
