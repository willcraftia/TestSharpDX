#region Using

using System;
using System.IO;
using Libra.Games;
using Libra.Games.SharpDX;
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
            var manager = new XnbTypeReaderManager();
            manager.LoadFrom(AppDomain.CurrentDomain);

            //var mapping = new XnbTypeReaderMapping();
            //mapping.LoadFrom(AppDomain.CurrentDomain);

            using (var stream = File.OpenRead("Content/dude.xnb"))
            {
                var reader = new XnbReader(stream, manager, Device);
                var model = reader.ReadXnb<object>();
                
                Console.WriteLine(model);
            }

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
