#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class IndexBufferBuilder : IndexBufferBuilderBase<IndexBuffer>
    {
        Device device;

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
            device = contentManager.Device as Device;

            base.Initialize(contentManager);
        }

        protected override void Begin(object deviceContext)
        {
            instance = new IndexBuffer(device);
        }

        protected override object End()
        {
            return instance;
        }
    }
}
