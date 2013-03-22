﻿#region Using

using System;
using Libra.Games;
using Libra.Games.SharpDX;
using Libra.Graphics;
using Libra.Graphics.Compiler;

using SDXWRenderForm = SharpDX.Windows.RenderForm;

#endregion

namespace Libra.Samples.MiniCubeIndexed
{
    public sealed class MainGame : Game
    {
        #region Vertices

        static readonly VertexPositionColor[] Vertices =
        {
            new VertexPositionColor(new Vector3(-1, -1,  1), new Color(255, 0, 0, 255)),
            new VertexPositionColor(new Vector3(-1,  1,  1), new Color(255, 0, 0, 255)),
            new VertexPositionColor(new Vector3( 1,  1,  1), new Color(255, 0, 0, 255)),
            new VertexPositionColor(new Vector3( 1, -1,  1), new Color(255, 0, 0, 255)),

            new VertexPositionColor(new Vector3( 1, -1, -1), new Color(0, 255, 0, 255)),
            new VertexPositionColor(new Vector3( 1,  1, -1), new Color(0, 255, 0, 255)),
            new VertexPositionColor(new Vector3(-1,  1, -1), new Color(0, 255, 0, 255)),
            new VertexPositionColor(new Vector3(-1, -1, -1), new Color(0, 255, 0, 255)),

            new VertexPositionColor(new Vector3(-1,  1,  1), new Color(0, 0, 255, 255)),
            new VertexPositionColor(new Vector3(-1,  1, -1), new Color(0, 0, 255, 255)),
            new VertexPositionColor(new Vector3( 1,  1, -1), new Color(0, 0, 255, 255)),
            new VertexPositionColor(new Vector3( 1,  1,  1), new Color(0, 0, 255, 255)),

            new VertexPositionColor(new Vector3( 1, -1,  1), new Color(255, 255, 0, 255)),
            new VertexPositionColor(new Vector3( 1, -1, -1), new Color(255, 255, 0, 255)),
            new VertexPositionColor(new Vector3(-1, -1, -1), new Color(255, 255, 0, 255)),
            new VertexPositionColor(new Vector3(-1, -1,  1), new Color(255, 255, 0, 255)),

            new VertexPositionColor(new Vector3(-1, -1, -1), new Color(255, 0, 255, 255)),
            new VertexPositionColor(new Vector3(-1,  1, -1), new Color(255, 0, 255, 255)),
            new VertexPositionColor(new Vector3(-1,  1,  1), new Color(255, 0, 255, 255)),
            new VertexPositionColor(new Vector3(-1, -1,  1), new Color(255, 0, 255, 255)),

            new VertexPositionColor(new Vector3( 1, -1,  1), new Color(0, 255, 255, 255)),
            new VertexPositionColor(new Vector3( 1,  1,  1), new Color(0, 255, 255, 255)),
            new VertexPositionColor(new Vector3( 1,  1, -1), new Color(0, 255, 255, 255)),
            new VertexPositionColor(new Vector3( 1, -1, -1), new Color(0, 255, 255, 255)),
        };

        #endregion

        #region Indices

        static readonly ushort[] Indices =
        {
            0,  1,  2,  0,  2,  3,
            4,  5,  6,  4,  6,  7,
            8,  9, 10,  8, 10, 11,
            12, 13, 14, 12, 14, 15,
            16, 17, 18, 16, 18, 19,
            20, 21, 22, 20, 22, 23,
        };

        #endregion

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
            inputLayout.Initialize<VertexPositionColor>(vsBytecode);

            vertexBuffer = Device.CreateVertexBuffer();
            vertexBuffer.Usage = ResourceUsage.Immutable;
            vertexBuffer.Initialize(Vertices);

            indexBuffer = Device.CreateIndexBuffer();
            indexBuffer.Usage = ResourceUsage.Immutable;
            indexBuffer.Initialize(Indices);

            constantBuffer = Device.CreateConstantBuffer();
            constantBuffer.Initialize<Matrix>();

            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            var context = Device.ImmediateContext;

            context.Clear(Color.CornflowerBlue);

            // テストのために入力レイアウト自動解決を OFF に設定。
            context.AutoResolveInputLayout = false;
            context.InputLayout = inputLayout;
            context.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.SetVertexBuffer(0, vertexBuffer);
            context.IndexBuffer = indexBuffer;

            context.VertexShader = vertexShader;
            context.PixelShader = pixelShader;
            context.SetVertexShaderConstantBuffer(0, constantBuffer);

            float aspect = context.Viewport.AspectRatio;
            float time = (float) gameTime.TotalGameTime.TotalSeconds;

            var world = Matrix.CreateRotationX(time) * Matrix.CreateRotationY(time * 2) * Matrix.CreateRotationZ(time * .7f);
            var view = Matrix.CreateLookAt(new Vector3(0, 0, -5), Vector3.Zero, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1f, 100.0f);

            var worldViewProjection = world * view * projection;
            // 列優先としているので転置してから設定。
            worldViewProjection.Transpose();
            constantBuffer.SetData(context, worldViewProjection);

            context.DrawIndexed(indexBuffer.IndexCount);

            base.Draw(gameTime);
        }
    }

    #region Program

    static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MainGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
