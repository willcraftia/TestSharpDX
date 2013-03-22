#region Using

using System;
using System.IO;
using Libra.Games;
using Libra.Games.SharpDX;
using Libra.Graphics;
using Libra.Content.Xnb;
using Libra.Input;

#endregion

namespace Libra.Samples.LoadXnb
{
    public sealed class MainGame : Game
    {
        IGamePlatform platform;

        GraphicsManager graphicsManager;

        Model model;

        IKeyboard keyboard;

        public MainGame()
        {
            platform = new SdxFormGamePlatform(this)
            {
                DirectInputEnabled = true
            };
            graphicsManager = new GraphicsManager(this);
        }

        protected override void Initialize()
        {
            var exists = System.IO.File.Exists("Content/dude.xnb");

            base.Initialize();
        }

        const string XnaModelReaderName = "";

        protected override void LoadContent()
        {
            var manager = new XnbManager(Device);
            manager.TypeReaderManager.LoadFrom(AppDomain.CurrentDomain);
            manager.RootDirectory = "Content";

            model = manager.Load<Model>("dude");

            var viewport = Device.ImmediateContext.Viewport;

            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = Matrix.CreateLookAt(
                        new Vector3(0.0f, 100.0f, 200.0f),
                        Vector3.Zero,
                        Vector3.Up
                    );

                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f),
                        (float) viewport.AspectRatio,
                        1.0f,
                        1000.0f
                    );
                }
            }

            keyboard = platform.CreateKeyboard();

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Device.ImmediateContext.Clear(Color.CornflowerBlue);

            foreach (var mesh in model.Meshes)
            {
                mesh.Draw(Device.ImmediateContext);
            }

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
