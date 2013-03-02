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

        IInputAssemblerStage InputAssemblerStage { get; }

        IVertexShaderStage VertexShaderStage { get; }

        IRasterizerStage RasterizerStage { get; }

        IPixelShaderStage PixelShaderStage { get; }

        IOutputMergerStage OutputMergerStage { get; }

        void Draw(int vertexCount, int startVertexLocation = 0);
    }
}
