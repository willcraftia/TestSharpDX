#region using

using System;

#endregion

namespace Libra.Samples.MiniCubeIndexed
{
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
}
