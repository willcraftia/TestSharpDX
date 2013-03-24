#region Using

using System;
using Felis.Xnb;
using Libra.Graphics;

#endregion

namespace Libra.Xnb
{
    public sealed class VertexBufferBuilder : VertexBufferBuilderBase<VertexBuffer>
    {
        IDevice device;

        VertexBuffer instance;

        VertexDeclaration vertexDeclaration;

        protected override void Initialize(ContentManager contentManager)
        {
            device = contentManager.Device as IDevice;

            base.Initialize(contentManager);
        }

        protected override void SetVertexDeclaration(object value)
        {
            vertexDeclaration = (VertexDeclaration) value;
        }

        protected override void SetVertexCount(uint value)
        {
        }

        protected override uint GetVertexStride()
        {
            return (uint) vertexDeclaration.Stride;
        }

        protected override void SetVertexData(byte[] value)
        {
            instance.Initialize(vertexDeclaration, value);
        }

        protected override void Begin(object deviceContext)
        {
            instance = device.CreateVertexBuffer();
        }

        protected override object End()
        {
            return instance;
        }
    }
}
