#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class VertexBufferBuilder : VertexBufferBuilderBase<VertexBuffer>
    {
        Device device;

        VertexBuffer instance;

        protected override void SetVertexDeclaration(object value)
        {
            instance.VertexDeclaration = (VertexDeclaration) value;
        }

        protected override void SetVertexCount(uint value)
        {
            instance.VertexCount = (int) value;
        }

        protected override uint GetVertexStride()
        {
            return (uint) instance.VertexDeclaration.VertexStride;
        }

        protected override void SetVertexData(byte[] value)
        {
            instance.VertexData = value;
        }

        protected override void Initialize(ContentManager contentManager)
        {
            device = contentManager.Device as Device;

            base.Initialize(contentManager);
        }

        protected override void Begin(object deviceContext)
        {
            instance = new VertexBuffer(device);
        }

        protected override object End()
        {
            return instance;
        }
    }
}
