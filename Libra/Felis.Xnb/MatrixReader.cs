#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public sealed class MatrixReader : TypeReader
    {
        MatrixBuilderBase builder;

        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Matrix"; }
        }

        public override string ReaderName
        {
            get { return "Microsoft.Xna.Framework.Content.MatrixReader"; }
        }

        public override bool IsValueType
        {
            get { return true; }
        }

        protected internal override void Initialize(TypeReaderManager manager)
        {
            builder = manager.GetTypeBuilderByTargetType(TargetType) as MatrixBuilderBase;

            base.Initialize(manager);
        }

        protected internal override object Read(ContentReader input)
        {
            builder.Begin();
            builder.SetValues(
                input.ReadSingle(), input.ReadSingle(), input.ReadSingle(), input.ReadSingle(),
                input.ReadSingle(), input.ReadSingle(), input.ReadSingle(), input.ReadSingle(),
                input.ReadSingle(), input.ReadSingle(), input.ReadSingle(), input.ReadSingle(),
                input.ReadSingle(), input.ReadSingle(), input.ReadSingle(), input.ReadSingle());
            return builder.End();
        }
    }
}
