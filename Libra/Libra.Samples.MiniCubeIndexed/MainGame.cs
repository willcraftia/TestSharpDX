#region Using

using System;
using Libra.Games;
using Libra.Games.SharpDX;
using Libra.Graphics;
using Libra.Compiler;

using SDXWRenderForm = SharpDX.Windows.RenderForm;

#endregion

namespace Libra.Samples.MiniCubeIndexed
{
    public sealed class MainGame : Game
    {
        IGamePlatform platform;

        GraphicsManager graphicsManager;

        VertexShader vertexShader;

        PixelShader pixelShader;

        InputLayout inputLayout;

        VertexBuffer vertexBuffer;

        IndexBuffer indexBuffer;

        ConstantBuffer constantBuffer;

        public MainGame()
        {
            platform = new SdxFormGamePlatform(this, new SDXWRenderForm());
            graphicsManager = new GraphicsManager(this);
        }

        protected override void LoadContent()
        {
            var compiler = new ShaderCompiler();
            compiler.RootPath = "Shaders";

            var vsBytecode = compiler.CompileFromFile("MiniCube.fx", "VS", VertexShaderProfile.vs_4_0);
            var psBytecode = compiler.CompileFromFile("MiniCube.fx", "PS", PixelShaderProfile.ps_4_0);

            vertexShader = Device.CreateVertexShader();
            vertexShader.Initialize(vsBytecode);

            pixelShader = Device.CreatePixelShader();
            pixelShader.Initialize(psBytecode);
            
            inputLayout = Device.CreateInputLayout();
            inputLayout.Initialize<InputPositionColor>(vsBytecode);

            vertexBuffer = Device.CreateVertexBuffer();
            vertexBuffer.Usage = ResourceUsage.Immutable;
            vertexBuffer.Initialize(new[]
                {
                    new InputPositionColor { Position = new Vector3(-1, -1,  1), Color = new Color(255, 0, 0, 255) },
                    new InputPositionColor { Position = new Vector3(-1,  1,  1), Color = new Color(255, 0, 0, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1,  1), Color = new Color(255, 0, 0, 255) },
                    new InputPositionColor { Position = new Vector3( 1, -1,  1), Color = new Color(255, 0, 0, 255) },

                    new InputPositionColor { Position = new Vector3( 1, -1, -1), Color = new Color(0, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1, -1), Color = new Color(0, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3(-1,  1, -1), Color = new Color(0, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3(-1, -1, -1), Color = new Color(0, 255, 0, 255) },

                    new InputPositionColor { Position = new Vector3(-1,  1,  1), Color = new Color(0, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3(-1,  1, -1), Color = new Color(0, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1, -1), Color = new Color(0, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1,  1), Color = new Color(0, 0, 255, 255) },

                    new InputPositionColor { Position = new Vector3( 1, -1,  1), Color = new Color(255, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3( 1, -1, -1), Color = new Color(255, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3(-1, -1, -1), Color = new Color(255, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3(-1, -1,  1), Color = new Color(255, 255, 0, 255) },

                    new InputPositionColor { Position = new Vector3(-1, -1, -1), Color = new Color(255, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3(-1,  1, -1), Color = new Color(255, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3(-1,  1,  1), Color = new Color(255, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3(-1, -1,  1), Color = new Color(255, 0, 255, 255) },

                    new InputPositionColor { Position = new Vector3( 1, -1,  1), Color = new Color(0, 255, 255, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1,  1), Color = new Color(0, 255, 255, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1, -1), Color = new Color(0, 255, 255, 255) },
                    new InputPositionColor { Position = new Vector3( 1, -1, -1), Color = new Color(0, 255, 255, 255) },
                });

            indexBuffer = Device.CreateIndexBuffer();
            indexBuffer.Usage = ResourceUsage.Immutable;
            indexBuffer.Initialize(new ushort[]
                {
                     0,  1,  2,  0,  2,  3,
                     4,  5,  6,  4,  6,  7,
                     8,  9, 10,  8, 10, 11,
                    12, 13, 14, 12, 14, 15,
                    16, 17, 18, 16, 18, 19,
                    20, 21, 22, 20, 22, 23,
                });

            constantBuffer = Device.CreateConstantBuffer();
            constantBuffer.Initialize<Matrix>();

            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            var context = Device.ImmediateContext;

            context.Clear(Color.CornflowerBlue);

            context.InputAssemblerStage.InputLayout = inputLayout;
            context.InputAssemblerStage.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssemblerStage.SetVertexBuffer<InputPositionColor>(0, vertexBuffer);
            context.InputAssemblerStage.IndexBuffer = indexBuffer;

            context.VertexShaderStage.VertexShader = vertexShader;
            context.VertexShaderStage.SetConstantBuffer(0, constantBuffer);
            context.PixelShaderStage.PixelShader = pixelShader;

            float aspect = context.RasterizerStage.Viewport.AspectRatio;
            float time = (float) gameTime.TotalGameTime.TotalSeconds;

            var world = Matrix.CreateRotationX(time) * Matrix.CreateRotationY(time * 2) * Matrix.CreateRotationZ(time * .7f);
            var view = Matrix.CreateLookAt(new Vector3(0, 0, -5), Vector3.Zero, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1f, 100.0f);

            var worldViewProjection = world * view * projection;
            // 列優先としているので転置してから設定。
            worldViewProjection.Transpose();
            constantBuffer.SetData(context, worldViewProjection);

            context.DrawIndexed(36);

            base.Draw(gameTime);
        }
    }
}
