#region Using

using System;
using System.IO;
using Libra.Games;
using Libra.Games.SharpDX;
using Libra.Graphics;
using Libra.Content.Xnb;

#endregion

namespace Libra.Samples.LoadXnb
{
    public sealed class MainGame : Game
    {
        IGamePlatform platform;

        GraphicsManager graphicsManager;

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

            var model = manager.Load<Model>("dude");
            Console.WriteLine(model);

            base.LoadContent();
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
