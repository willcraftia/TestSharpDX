#region Using

using System;
using System.Text;

#endregion

namespace Libra.Graphics
{
    public sealed class VertexDeclaration
    {
        const int InputSlotCount = D3D11Constants.IAVertexInputResourceSlotCount;

        VertexElement[] elements;

        public int Stride { get; private set; }

        public VertexDeclaration(params VertexElement[] elements)
        {
            if (elements == null) throw new ArgumentNullException("elements");
            if (elements.Length == 0) throw new ArgumentException("elements is empty", "elements");

            this.elements = elements;

            foreach (var element in elements)
            {
                Stride += element.SizeInBytes;
            }
        }

        public VertexDeclaration(int stride, params VertexElement[] elements)
        {
            if (stride < 1) throw new ArgumentOutOfRangeException("stride");
            if (elements == null) throw new ArgumentNullException("elements");
            if (elements.Length == 0) throw new ArgumentException("elements is empty", "elements");

            Stride = stride;
            this.elements = elements;
        }

        public VertexElement[] GetVertexElements()
        {
            return (VertexElement[]) elements.Clone();
        }

        public InputElement[] GetInputElements(int slot)
        {
            if ((uint) InputSlotCount < (uint) slot) throw new ArgumentOutOfRangeException("slot");

            var inputElements = new InputElement[elements.Length];

            for (int i = 0; i < elements.Length; i++)
            {
                inputElements[i] = new InputElement(
                    elements[i].SemanticName,
                    elements[i].SemanticIndex,
                    elements[i].Format,
                    slot,
                    elements[i].AlignedByteOffset,
                    elements[i].PerInstance,
                    elements[i].InstanceDataStepRate);
            }

            return inputElements;
        }
    }
}
