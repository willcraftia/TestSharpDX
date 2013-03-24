#region Using

using System;
using Felis.Xnb;
using Libra.Graphics;

#endregion

namespace Libra.Xnb
{
    public sealed class Texture2DBuilder : Texture2DBuilderBase<Texture2D>
    {
        IDevice device;

        DeviceContext currentContext;

        Texture2D instance;

        int currentMipLevel;

        protected override void Initialize(ContentManager contentManager)
        {
            device = contentManager.Device as IDevice;

            base.Initialize(contentManager);
        }

        protected override void SetSurfaceFormat(int value)
        {
            instance.Format = SurfaceFormatConverter.ToSurfaceFormat(value);
        }

        protected override void SetWidth(uint value)
        {
            instance.Width = (int) value;
        }

        protected override void SetHeight(uint value)
        {
            instance.Height = (int) value;
        }

        protected override void SetMipCount(uint value)
        {
            instance.MipLevels = (int) value;
        }

        protected override void BeginMips()
        {
            instance.Initialize();

            base.BeginMips();
        }

        protected override void BeginMip(int index)
        {
            currentMipLevel = index;
        }

        protected override void SetMipDataSize(uint value)
        {
        }

        protected override void SetMipImageData(byte[] value)
        {
            instance.SetData(currentContext, currentMipLevel, value);
        }

        protected override void Begin(object deviceContext)
        {
            currentContext = deviceContext as DeviceContext;
            if (currentContext == null) currentContext = device.ImmediateContext;

            instance = device.CreateTexture2D();
        }

        protected override object End()
        {
            return instance;
        }
    }
}
