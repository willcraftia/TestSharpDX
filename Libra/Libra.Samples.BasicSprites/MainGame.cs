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

            var processorProperties = new System.Collections.Generic.Dictionary<string, object>();
            processorProperties["HasHiragana"] = true;
            processorProperties["HasKatakana"] = true;
            processorProperties["Text"] = "漢字可能";

            var outputPath = compiler.Compile("Fonts/SpriteFont.json", "FontDescriptionProcessor", processorProperties);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = Device.CreateTexture2D();
            texture.Usage = ResourceUsage.Immutable;
            texture.Initialize("Textures/Libra.png");

            textureView = Device.CreateShaderResourceView();
            textureView.Initialize(texture);

            spriteBatch = new SpriteBatch(Device.ImmediateContext);

            var loaderFactory = new ContentLoaderFactory(Device, AppDomain.CurrentDomain);
            var loader = loaderFactory.CreateLoader();

            spriteFont = loader.Load<SpriteFont>("Fonts/SpriteFont");

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
            spriteBatch.DrawString(spriteFont, "ひらがなカタカナ漢字可能", new Vector2(0, 0), Color.White);
            //spriteBatch.DrawString(spriteFont, "RGB", new Vector2(0, 140), Color.Red);
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
