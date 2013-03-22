#region Using

using System;
using System.IO;

#endregion

namespace Libra.Graphics
{
    public abstract class RenderTarget : Texture2D
    {
        public DepthFormat DepthFormat { get; set; }

        public RenderTargetUsage RenderTargetUsage { get; set; }

        public DepthStencil DepthStencil { get; protected set; }

        protected RenderTarget(IDevice device)
            : base(device)
        {
            DepthFormat = DepthFormat.None;
            RenderTargetUsage = RenderTargetUsage.Discard;
        }

        public sealed override void Initialize()
        {
            InitializeRenderTarget();

            if (DepthFormat != DepthFormat.None)
                DepthStencil = InitializeDepthStencil();
        }

        public sealed override void Initialize(Stream stream)
        {
            InitializeRenderTarget(stream);

            if (DepthFormat != DepthFormat.None)
                DepthStencil = InitializeDepthStencil();
        }

        protected abstract void InitializeRenderTarget();

        protected abstract void InitializeRenderTarget(Stream stream);

        protected abstract DepthStencil InitializeDepthStencil();

        protected override void DisposeOverride(bool disposing)
        {
            if (DepthStencil != null)
                DepthStencil.Dispose();

            base.DisposeOverride(disposing);
        }
    }
}
