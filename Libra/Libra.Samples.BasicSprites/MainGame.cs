#region Using

using System;
using Libra.Content;
using Libra.Games;
using Libra.Games.SharpDX;
using Libra.Graphics;
using Libra.Input;

using Libra.Content.Pipeline.Compiler;
using Libra.Content.Pipeline.Processors;
using Libra.Content.Serialization;

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
            graphicsManager.PreferredBackBufferWidth = 800;
            graphicsManager.PreferredBackBufferHeight = 300;
        }

        protected override void Initialize()
        {
            var compilerFactory = new ContentCompilerFactory(AppDomain.CurrentDomain);
            compilerFactory.SourceRootDirectory = "../../";

            var compiler = compilerFactory.CreateCompiler();

            var processorProperties = new System.Collections.Generic.Dictionary<string, object>();
            processorProperties["HasAsciiCharacters"] = false;
            processorProperties["HasHiragana"] = true;
            processorProperties["HasKatakana"] = true;
            processorProperties["Text"] = "漢字可能(*´∀｀*)ー";
            processorProperties["OutlineThickness"] = 3.0f;
            processorProperties["OutlineColor"] = Color.Black;
            processorProperties["OutlineShape"] = System.Windows.Media.PenLineJoin.Bevel;
            // TODO
            // グラデーションが効いていない。
            processorProperties["UseGradient"] = true;
            processorProperties["GradientBeginColor"] = Color.Navy;
            processorProperties["GradientEndColor"] = Color.LightBlue;

            var outputPath = compiler.Compile("Fonts/SpriteFont.json", "JsonFontSerializer", "FontDescriptionProcessor", processorProperties);

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
            spriteBatch.Draw(textureView, Vector2.Zero, Color.White);
            spriteBatch.Draw(textureView, new Vector2(128, 0), Color.Red);
            spriteBatch.Draw(textureView, new Vector2(128 * 2, 0), null, Color.White,
                0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally);
            spriteBatch.Draw(textureView, new Vector2(128 * 3, 0), null, Color.White,
                0, Vector2.Zero, 1, SpriteEffects.FlipVertically);
            spriteBatch.Draw(textureView, new Vector2(128 * 4, 0), null, Color.White,
                0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically);
            spriteBatch.DrawString(spriteFont, "ひらがなカタカナ漢字可能(*´∀｀*)", new Vector2(0, 128), Color.White);
            spriteBatch.DrawString(spriteFont, "スケール", new Vector2(0, 128 + 64), Color.White, 0, Vector2.Zero, 0.5f);
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
