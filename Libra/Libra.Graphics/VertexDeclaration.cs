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

        internal string Signature { get; private set; }

        public VertexDeclaration(params InputElement[] elements)
        {
            if (elements == null) throw new ArgumentNullException("elements");
            if (elements.Length == 0) throw new ArgumentException("elements is empty", "elements");

            this.elements = elements;

            var builder = new StringBuilder();
            foreach (var element in elements)
            {
                Stride += element.SizeInBytes;

                builder.Append(element.SemanticName);
                builder.Append(element.SemanticIndex);
                builder.Append((int) element.Format);
                builder.Append(element.InputSlot);
                builder.Append(element.AlignedByteOffset);
                builder.Append(element.PerInstance ? 1 : 0);
                builder.Append(element.InstanceDataStepRate);
            }
            Signature = builder.ToString();
        }

        public VertexDeclaration(int stride, params InputElement[] elements)
        {
            if (stride < 1) throw new ArgumentOutOfRangeException("stride");
            if (elements == null) throw new ArgumentNullException("elements");
            if (elements.Length == 0) throw new ArgumentException("elements is empty", "elements");

            Stride = stride;
            this.elements = elements;

            var builder = new StringBuilder();
            foreach (var element in elements)
            {
                builder.Append(element.SemanticName);
                builder.Append(element.SemanticIndex);
                builder.Append((int) element.Format);
                builder.Append(element.InputSlot);
                builder.Append(element.AlignedByteOffset);
                builder.Append(element.PerInstance ? 1 : 0);
                builder.Append(element.InstanceDataStepRate);
            }
            Signature = builder.ToString();
        }
    }
}
