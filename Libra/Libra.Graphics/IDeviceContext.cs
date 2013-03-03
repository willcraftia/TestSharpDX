#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IDeviceContext : IDisposable
    {
        event EventHandler Disposing;

        IDevice Device { get; }

        bool Deferred { get; }

        InputAssemblerStage InputAssemblerStage { get; }

        VertexShaderStage VertexShaderStage { get; }

        RasterizerStage RasterizerStage { get; }

        PixelShaderStage PixelShaderStage { get; }

        OutputMergerStage OutputMergerStage { get; }

        void Draw(int vertexCount, int startVertexLocation = 0);
    }
}
