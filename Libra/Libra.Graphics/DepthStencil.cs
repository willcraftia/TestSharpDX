#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class DepthStencil : Resource
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public DepthFormat Format { get; set; }

        public int MultisampleCount { get; set; }

        public int MultisampleQuality { get; set; }

        protected DepthStencil(IDevice device)
            : base(device)
        {
            Format = DepthFormat.Depth24Stencil8;
            MultisampleCount = 1;
            MultisampleQuality = 0;
        }

        public abstract void Initialize();
    }
}
