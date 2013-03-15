#region Using

using System;
using Libra.Games;
using Libra.Games.SharpDX;
using Libra.Graphics;
using Libra.Input;

using Libra.Content.Pipeline.Compiler;

#endregion

namespace Libra.Samples.BasicSprites
{
    public sealed class MainGame : Game
    {
        IGamePlatform platform;

        GraphicsManager graphicsManager;

        Texture2D texture;

        ShaderResourceView textureView;

        SpriteBatch spriteBatch;

        IKeyboard keyboard;

        KeyboardState currentKeyboardState;

        KeyboardState lastKeyboardState;

        public MainGame()
        {
            platform = new SdxFormGamePlatform(this);
            graphicsManager = new GraphicsManager(this);
        }

        protected override void Initialize()
        {
            var compilerFactory = new ContentCompilerFactory(AppDomain.CurrentDomain);
            var compiler = compilerFactory.CreateCompiler();
            var outputPath = compiler.Compile("Fonts/SpriteFont.json", "FontDescriptionProcessor");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = Device.CreateTexture2D();
            texture.Usage = ResourceUsage.Immutable;
            texture.Initialize("Textures/Libra.png");
            //texture.Width = 1;
            //texture.Height = 1;
            //texture.Initialize();
            //texture.SetData(Device.ImmediateContext, Color.White);

            textureView = Device.CreateShaderResourceView();
            textureView.Initialize(texture);

            spriteBatch = new SpriteBatch(Device.ImmediateContext);

            keyboard = platform.CreateKeyboard();

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Escape) && lastKeyboardState.IsKeyUp(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var context = Device.ImmediateContext;

            context.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(textureView, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    #region Program

    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
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
