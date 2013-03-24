#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class VertexDeclarationBuilder : VertexDeclarationBuilderBase<VertexDeclaration>
    {
        VertexDeclaration instance;

        int currentElementIndex;

        protected override void SetVertexStride(uint value)
        {
            instance.VertexStride = (int) value;
        }

        protected override void SetElementCount(uint value)
        {
            instance.Elements = new VertexElement[value];
        }

        protected override void BeginElement(int index)
        {
            currentElementIndex = index;
        }

        protected override void SetElementOffset(uint value)
        {
            instance.Elements[currentElementIndex].Offset = (int) value;
        }

        protected override void SetElementFormat(int value)
        {
            instance.Elements[currentElementIndex].Format = value;
        }

        protected override void SetElementUsage(int value)
        {
            instance.Elements[currentElementIndex].Usage = value;
        }

        protected override void SetElementUsageIndex(uint value)
        {
            instance.Elements[currentElementIndex].UsageIndex = (int) value;
        }

        protected override void Begin(object deviceContext)
        {
            instance = new VertexDeclaration();
        }

        protected override object End()
        {
            return instance;
        }
    }
}
