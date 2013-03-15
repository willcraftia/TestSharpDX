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
            var factory = new ContentCompilerFactory();

            factory.Serializers.FindAndAddFrom(AppDomain.CurrentDomain);
            factory.ProcessorTypes.FindAndAddFrom(AppDomain.CurrentDomain);

            var compiler = factory.CreateCompiler();

            var outputPath = compiler.Compile("SpriteFont.json", "FontDescriptionProcessor", null);

            Console.WriteLine("outputPath: {0}", outputPath);
            
            Console.WriteLine("Press enter key to exit...");
            Console.ReadLine();
        }
    }
}
