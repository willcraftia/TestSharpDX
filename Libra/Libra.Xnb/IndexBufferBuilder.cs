#region Using

using System;
using Felis.Xnb;
using Libra.Graphics;

#endregion

namespace Libra.Xnb
{
    public sealed class IndexBufferBuilder : IndexBufferBuilderBase<IndexBuffer>
    {
        IDevice device;

        IndexBuffer instance;

        protected override void Initialize(ContentManager contentManager)
        {
            device = contentManager.Device as IDevice;

            base.Initialize(contentManager);
        }

        protected override void SetIsSixteenBits(bool value)
        {
            instance.Format = (value) ? IndexFormat.SixteenBits : IndexFormat.ThirtyTwoBits;
        }

        protected override void SetDataSize(uint value)
        {
        }

        protected override void SetIndexData(byte[] value)
        {
            instance.Initialize(value);
        }

        protected override void Begin(object deviceContext)
        {
            instance = device.CreateIndexBuffer();
        }

        protected override object End()
        {
            return instance;
        }
    }
}
