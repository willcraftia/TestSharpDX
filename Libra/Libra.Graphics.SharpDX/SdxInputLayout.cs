﻿#region Using

using System;
using System.Collections.Generic;

using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11InputClassification = SharpDX.Direct3D11.InputClassification;
using D3D11InputElement = SharpDX.Direct3D11.InputElement;
using D3D11InputLayout = SharpDX.Direct3D11.InputLayout;
using DXGIFormat = SharpDX.DXGI.Format;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxInputLayout : InputLayout
    {
        public D3D11Device D3D11Device { get; private set; }

        public D3D11InputLayout D3D11InputLayout { get; private set; }

        public SdxInputLayout(SdxDevice device)
            : base(device)
        {
            D3D11Device = device.D3D11Device;
        }

        protected override void InitializeCore(byte[] shaderBytecode, InputElement[] inputElements)
        {
            if (shaderBytecode == null) throw new ArgumentNullException("shaderBytecode");
            if (inputElements == null) throw new ArgumentNullException("inputElements");

            var d3d11InputElements = new D3D11InputElement[inputElements.Length];
            for (int i = 0; i < inputElements.Length; i++)
            {
                var d3d11InputClassification = D3D11InputClassification.PerVertexData;
                if (inputElements[i].PerInstance)
                    d3d11InputClassification = D3D11InputClassification.PerInstanceData;

                d3d11InputElements[i] = new D3D11InputElement
                {
                    SemanticName = inputElements[i].SemanticName,
                    SemanticIndex = inputElements[i].SemanticIndex,
                    Format = (DXGIFormat) inputElements[i].Format,
                    Slot = inputElements[i].InputSlot,
                    AlignedByteOffset = inputElements[i].AlignedByteOffset,
                    Classification = d3d11InputClassification,
                    InstanceDataStepRate = inputElements[i].InstanceDataStepRate
                };
            }

            // シグネチャ生成無しでも入力レイアウトを生成可能。
            // このため、シグネチャを用いるならば、
            // クラス外部でこれを生成し、引数 shaderBytecode に指定すれば良い。
            D3D11InputLayout = new D3D11InputLayout(D3D11Device, shaderBytecode, d3d11InputElements);
        }

        #region IDisposable

        protected override void DisposeOverride(bool disposing)
        {
            if (disposing)
            {
                if (D3D11InputLayout != null)
                    D3D11InputLayout.Dispose();
            }

            base.DisposeOverride(disposing);
        }

        #endregion
    }
}
