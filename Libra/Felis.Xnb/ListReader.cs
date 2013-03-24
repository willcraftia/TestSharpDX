#region Using

using System;

#endregion

namespace Felis.Xnb
{
    [GenericTypeReader("System.Collections.Generic.List", "Microsoft.Xna.Framework.Content.ListReader")]
    public sealed class ListReader : GenericTypeReader
    {
        ListBuilder builder;

        TypeReader itemReader;

        protected internal override void Initialize(TypeReaderManager manager)
        {
            builder = manager.GetTypeBuilderByTargetType(TargetType) as ListBuilder;

            itemReader = manager.GetTypeReaderByTargetType(GenericArguments[0]);

            base.Initialize(manager);
        }

        protected internal override object Read(ContentReader input)
        {
            builder.Begin();

            // Count
            var count = input.ReadUInt32();
            builder.SetCount(count);

            // Repeat <count>
            for (int i = 0; i < count; i++)
            {
                builder.Add(itemReader.Read(input));
            }

            return builder.End();
        }
    }
}
