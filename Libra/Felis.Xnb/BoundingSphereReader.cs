#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public sealed class BoundingSphereReader : TypeReader
    {
        BoundingSphereBuilderBase builder;

        Vector3Reader vector3Reader;

        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.BoundingSphere"; }
        }

        public override string ReaderName
        {
            get { return "Microsoft.Xna.Framework.Content.BoundingSphereReader"; }
        }

        protected internal override void Initialize(TypeReaderManager manager)
        {
            builder = manager.GetTypeBuilderByTargetType(TargetType) as BoundingSphereBuilderBase;

            vector3Reader = new Vector3Reader();
            vector3Reader.Initialize(manager);

            base.Initialize(manager);
        }

        protected internal override object Read(ContentReader input)
        {
            builder.Begin(input.DeviceContext);

            // Center
            builder.SetCenter(vector3Reader.Read(input));

            // Radius
            builder.SetRadius(input.ReadSingle());

            return builder.End();
        }
    }
}
