#region Using

using System;
using System.Collections.Generic;
using System.IO;
using Libra.Content.Pipeline;
using Libra.Content.Pipeline.Compiler;
using Libra.Content.Pipeline.Processors;
using Libra.Content.Serialization;

#endregion

namespace Libra.Samples.ContentCompile
{
    class Program
    {
        static void Main(string[] args)
        {
            var compilerFactory = new ContentCompilerFactory(AppDomain.CurrentDomain);
            
            // 元ファイルを実行時ディレクトリ (bin/Debug や bin/Release) へコピーしない場合は、
            // 実行時ディレクトリからの相対パスを指定する必要がある。
            compilerFactory.SourceRootDirectory = "../../";

            var compiler = compilerFactory.CreateCompiler();

            var sourcePath = "SpriteFont.json";

            var processorProperties = new Properties();
            processorProperties["HasAsciiCharacters"] = false;
            processorProperties["HasHiragana"] = true;
            processorProperties["HasKatakana"] = true;
            processorProperties["Text"] = "明示的に追加する文字";

            var serializer = new JsonFontSerializer();
            var processor = new FontDescriptionProcessor
            {
                HasAsciiCharacters = false,
                HasHiragana = true,
                HasKatakana = true,
                Text = "明示的に追加する文字"
            };

            string outputPath;

            //----------------------------------------------------------------
            // シリアライザとプロセッサを名前で指定するパターン。
            // ファクトリ内にシリアライザとプロセッサの情報が設定されている必要がある。
            
            outputPath = compiler.Compile(sourcePath, "JsonFontSerializer", "FontDescriptionProcessor", processorProperties);
            Console.WriteLine("By names: {0}", outputPath);

            //----------------------------------------------------------------
            // シリアライザとプロセッサをジェネリクスで指定するパターン。
            // 型を明示するためファクトリ内にシリアライザとプロセッサの情報が設定されていなくて良いが、
            // プロセッサのプロパティを設定できない。

            outputPath = compiler.Compile<JsonFontSerializer, FontDescriptionProcessor>(sourcePath, processorProperties);
            Console.WriteLine("By generics: {0}", outputPath);

            //----------------------------------------------------------------
            // シリアライザとプロセッサをインスタンス化して指定するパターン。
            // 型を明示するためファクトリ内にシリアライザとプロセッサの情報が設定されていなくて良い。
            // プロセッサのプロパティはインスタンス化時に明示。

            outputPath = compiler.Compile(sourcePath, serializer, processor);
            Console.WriteLine("By explicit instances: {0}", outputPath);

            //----------------------------------------------------------------
            // Exit

            Console.WriteLine("Press enter key to exit...");
            Console.ReadLine();
        }
    }
}
