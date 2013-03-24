#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public sealed class Vector3Reader : TypeReader
    {
        Vector3BuilderBase builder;

        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Vector3"; }
        }

        public override string ReaderName
        {
            get { return "Microsoft.Xna.Framework.Content.Vector3Reader"; }
        }

        public override bool IsValueType
        {
            get { return true; }
        }

        protected internal override void Initialize(TypeReaderManager manager)
        {
            builder = manager.GetTypeBuilderByTargetType(TargetType) as Vector3BuilderBase;

            base.Initialize(manager);
        }

        protected internal override object Read(ContentReader input)
        {
            builder.Begin(input.DeviceContext);
            builder.SetValues(input.ReadSingle(), input.ReadSingle(), input.ReadSingle());
            return builder.End();
        }
    }
}
