#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public sealed class RectangleReader : TypeReader
    {
        RectangleBuilderBase builder;

        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Rectangle"; }
        }

        public override string ReaderName
        {
            get { return "Microsoft.Xna.Framework.Content.RectangleReader"; }
        }

        protected internal override void Initialize(TypeReaderManager manager)
        {
            builder = manager.GetTypeBuilderByTargetType(TargetType) as RectangleBuilderBase;

            base.Initialize(manager);
        }

        protected internal override object Read(XnbReader input)
        {
            builder.Begin();
            builder.SetValues(input.ReadInt32(), input.ReadInt32(), input.ReadInt32(), input.ReadInt32());
            return builder.End();
        }
    }
}
