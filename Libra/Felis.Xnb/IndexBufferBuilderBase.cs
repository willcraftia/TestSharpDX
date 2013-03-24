#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class IndexBufferBuilderBase : TypeBuilder
    {
        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.IndexBuffer"; }
        }

        public abstract void SetIsSixteenBits(bool value);

        public abstract void SetDataSize(uint value);

        public abstract void SetIndexData(byte[] value);
    }
}
