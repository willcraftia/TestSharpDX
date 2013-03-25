#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class IndexBufferBuilder : IndexBufferBuilderBase<IndexBuffer>
    {
        IGraphicsDeviceService graphicsDeviceService;

        IndexBuffer instance;

        protected override void SetIsSixteenBits(bool value)
        {
            instance.IsSixteenBits = value;
        }

        protected override void SetDataSize(uint value)
        {
            instance.DataSize = (int) value;
        }

        protected override void SetIndexData(byte[] value)
        {
            instance.IndexData = value;
        }

        protected override void Initialize(ContentManager contentManager)
        {
            graphicsDeviceService = contentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;

            base.Initialize(contentManager);
        }

        protected override void Begin(object deviceContext)
        {
            instance = new IndexBuffer(graphicsDeviceService.GraphicsDevice);
        }

        protected override object End()
        {
            return instance;
        }
    }
}
