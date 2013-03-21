#region Using

using System;
using System.Text;

#endregion

namespace Libra.Graphics
{
    public sealed class VertexDeclaration
    {
        InputElement[] elements;

        public int Stride { get; private set; }

        public InputElement[] Elements
        {
            get { return (InputElement[]) elements.Clone(); }
        }

        public VertexDeclaration(params InputElement[] elements)
        {
            if (elements == null) throw new ArgumentNullException("elements");
            if (elements.Length == 0) throw new ArgumentException("elements is empty", "elements");

            this.elements = elements;

            foreach (var element in elements)
            {
                Stride += element.SizeInBytes;
            }
        }

        public VertexDeclaration(int stride, params InputElement[] elements)
        {
            if (stride < 1) throw new ArgumentOutOfRangeException("stride");
            if (elements == null) throw new ArgumentNullException("elements");
            if (elements.Length == 0) throw new ArgumentException("elements is empty", "elements");

            Stride = stride;
            this.elements = elements;
        }
    }
}
