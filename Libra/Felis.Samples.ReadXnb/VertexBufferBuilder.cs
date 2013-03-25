#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class VertexBufferBuilder : VertexBufferBuilderBase<VertexBuffer>
    {
        IGraphicsDeviceService graphicsDeviceService;

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
            graphicsDeviceService = contentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;

            base.Initialize(contentManager);
        }

        protected override void Begin(object deviceContext)
        {
            instance = new VertexBuffer(graphicsDeviceService.GraphicsDevice);
        }

        protected override object End()
        {
            return instance;
        }
    }
}
