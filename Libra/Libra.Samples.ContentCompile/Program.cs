#region Using

using System;
using System.IO;
using Libra.Content.Pipeline.Compiler;

#endregion

namespace Libra.Samples.ContentCompile
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ContentCompilerFactory(AppDomain.CurrentDomain);
            var compiler = factory.CreateCompiler();

            var outputPath = compiler.Compile("SpriteFont.json", "FontDescriptionProcessor");

            Console.WriteLine("outputPath: {0}", outputPath);
            
            Console.WriteLine("Press enter key to exit...");
            Console.ReadLine();
        }
    }
}
