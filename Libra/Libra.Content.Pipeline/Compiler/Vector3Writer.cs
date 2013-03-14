#region Using

using System;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public sealed class Vector3Writer : ContentTypeWriter<Vector3>
    {
        protected internal override void Write(ContentWriter output, Vector3 value)
        {
            output.Write(value.X);
            output.Write(value.Y);
            output.Write(value.Z);
        }
    }
}
