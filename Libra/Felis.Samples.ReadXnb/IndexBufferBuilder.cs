#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class IndexBufferBuilder : IndexBufferBuilderBase
    {
        IndexBuffer instance;

        public override Type ActualType
        {
            get { return typeof(IndexBuffer); }
        }

        public override void SetIsSixteenBits(bool value)
        {
            instance.IsSixteenBits = value;
        }

        public override void SetDataSize(uint value)
        {
            instance.DataSize = (int) value;
        }

        public override void SetIndexData(byte[] value)
        {
            instance.IndexData = value;
        }

        public override void Begin()
        {
            instance = new IndexBuffer();
        }

        public override object End()
        {
            return instance;
        }
    }
}
