#region Using

using System;
using Libra.Content;
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

        SpriteFont spriteFont;

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
            compilerFactory.SourceRootDirectory = "../../";
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

            var loaderFactory = new ContentLoaderFactory(Device, AppDomain.CurrentDomain);
            var loader = loaderFactory.CreateLoader();

            spriteFont = loader.Load<SpriteFont>("Fonts/SpriteFont");

            {
                var texture2D = spriteFont.texture.Resource as Texture2D;

                using (var stream = System.IO.File.Create("Test.png"))
                {
                    texture2D.Save(Device.ImmediateContext, stream);
                }

                var colors = new Color[texture2D.Width * texture2D.Height];
                texture2D.GetData(Device.ImmediateContext, colors);

                Console.WriteLine();
            }

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
            //spriteBatch.Draw(textureView, Vector2.Zero, Color.White);
            //spriteBatch.Draw(textureView, new Vector2(128, 0), Color.Red);
            spriteBatch.Draw(spriteFont.texture, Vector2.Zero, Color.White);
            //spriteBatch.DrawString(spriteFont, "ABCDEFG", new Vector2(0, 128), Color.White);
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
