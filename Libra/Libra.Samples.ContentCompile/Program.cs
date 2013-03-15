#region Using

using System;
using System.IO;
using Libra.Content.Pipeline.Build;
using Libra.Content.Pipeline.Compiler;
using Libra.Content.Serialization;

#endregion

namespace Libra.Samples.ContentCompile
{
    class Program
    {
        static void Main(string[] args)
        {
            //var jsonSerializer = new JsonFontSerializer();

            //var fontDescription = new FontDescription
            //{
            //    FontName = "メイリオ",
            //    Size = 14,
            //    Style = FontDescriptionStyle.Regular,
            //};

            //using (var stream = File.Create("Test.json"))
            //{
            //    jsonSerializer.Serialize(stream, fontDescription);
            //}

            //FontDescription result;
            //using (var stream = File.OpenRead("Test.json"))
            //{
            //    result = jsonSerializer.Deserialize(stream) as FontDescription;
            //}

            var project = new ContentProject();

            var projectDescription = new ContentProjectDescription
            {
                OutputDirectory = "",
                DetectSerializers = true,
                DetectProcessors = true
            };

            projectDescription.BuildTargets.Add(new BuildTargetItem
            {
                Path = "SpriteFont.json",
                ProcessorName = "FontDescriptionProcessor",
            });

            project.Initialize(projectDescription);
            project.Build();

        }
    }
}
