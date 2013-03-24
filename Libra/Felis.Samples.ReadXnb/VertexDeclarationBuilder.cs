#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class VertexDeclarationBuilder : VertexDeclarationBuilderBase
    {
        VertexDeclaration instance;

        int currentElementIndex;

        public override Type ActualType
        {
            get { return typeof(VertexDeclaration); }
        }

        public override void SetVertexStride(uint value)
        {
            instance.VertexStride = (int) value;
        }

        public override void SetElementCount(uint value)
        {
            instance.Elements = new VertexElement[value];
        }

        public override void BeginElement(int index)
        {
            currentElementIndex = index;
        }

        public override void SetElementOffset(uint value)
        {
            instance.Elements[currentElementIndex].Offset = (int) value;
        }

        public override void SetElementFormat(int value)
        {
            instance.Elements[currentElementIndex].Format = value;
        }

        public override void SetElementUsage(int value)
        {
            instance.Elements[currentElementIndex].Usage = value;
        }

        public override void SetElementUsageIndex(uint value)
        {
            instance.Elements[currentElementIndex].UsageIndex = (int) value;
        }

        public override void Begin()
        {
            instance = new VertexDeclaration();
        }

        public override object End()
        {
            return instance;
        }
    }
}
