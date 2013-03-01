#region Using

using System;

using D3D11InputClassification = SharpDX.Direct3D11.InputClassification;
using D3D11InputElement = SharpDX.Direct3D11.InputElement;
using D3D11InputLayout = SharpDX.Direct3D11.InputLayout;
using DXGIFormat = SharpDX.DXGI.Format;

#endregion

namespace Libra.Graphics
{
    public sealed class InputLayout
    {
        public Device Device { get; private set; }

        public int InputStride { get; private set; }

        internal D3D11InputLayout D3D11InputLayout { get; private set; }

        public InputLayout(Device device, byte[] shaderBytecode, Type type)
        {
            if (device == null) throw new ArgumentNullException("device");
            if (shaderBytecode == null) throw new ArgumentNullException("shaderBytecode");
            if (type == null) throw new ArgumentNullException("type");
            
            if (!type.IsValueType)
                throw new ArgumentException("Type must be ValueType", "type");
            if (!typeof(IInputType).IsAssignableFrom(type))
                throw new ArgumentException("Type must implement IInputType", "type");

            var inputType = (IInputType) Activator.CreateInstance(type);
            var inputElements = inputType.GetInputElements();

            Initialize(device, shaderBytecode, inputElements);
        }

        public InputLayout(Device device, byte[] shaderBytecode, params InputElement[] elements)
        {
            if (device == null) throw new ArgumentNullException("device");
            if (shaderBytecode == null) throw new ArgumentNullException("shaderBytecode");
            if (elements == null) throw new ArgumentNullException("elements");
            if (elements.Length == 0) throw new ArgumentException("InputElement[] must be not empty.", "elements");

            Initialize(device, shaderBytecode, elements);
        }

        void Initialize(Device device, byte[] shaderBytecode, InputElement[] elements)
        {
            Device = device;

            InputStride = 0;

            var d3d11InputElements = new D3D11InputElement[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                d3d11InputElements[i] = new D3D11InputElement
                {
                    SemanticName = elements[i].SemanticName,
                    SemanticIndex = elements[i].SemanticIndex,
                    Format = (DXGIFormat) elements[i].Format,
                    Slot = elements[i].InputSlot,
                    AlignedByteOffset = elements[i].AlignedByteOffset,
                    Classification = ResolveD3D11InputClassification(ref elements[i]),
                    InstanceDataStepRate = elements[i].InstanceDataStepRate
                };

                InputStride += FormatHelper.SizeOfInBytes(elements[i].Format);
            }

            // シグネチャ生成無しでも入力レイアウトを生成可能。
            // このため、シグネチャを用いるならば、
            // クラス外部でこれを生成し、引数 shaderBytecode に指定すれば良い。
            D3D11InputLayout = new D3D11InputLayout(Device.D3D11Device, shaderBytecode, d3d11InputElements);
        }

        D3D11InputClassification ResolveD3D11InputClassification(ref InputElement element)
        {
            if (element.PerInstance)
                return D3D11InputClassification.PerInstanceData;

            return D3D11InputClassification.PerVertexData;
        }
    }
}
