#region Using

using System;

#endregion

namespace Libra.Content
{
    [ContentTypeReader]
    public sealed class Vector3Reader : ContentTypeReader<Vector3>
    {
        protected internal override Vector3 Read(ContentReader input, Vector3 existingInstance)
        {
            return new Vector3(input.ReadSingle(), input.ReadSingle(), input.ReadSingle());
        }
    }
}
