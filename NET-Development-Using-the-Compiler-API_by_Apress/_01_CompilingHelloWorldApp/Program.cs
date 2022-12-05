using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public class Program
{
  public static void Main()
  {
    // 実行したいコード
    var code = @"
    using System;

    namespace HelloWorld
    {
      public class Program
      {
        public static void Main()
        {
          Console.WriteLine(""Hello compiled world"");
        }
      }
    }";
    
    // ツリー構文
    var tree = SyntaxFactory.ParseSyntaxTree(code);

    // 参照情報
    List<MetadataReference> references = new ();
    {
      // ベースdll
      var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
      references.AddRange(new [] {
        MetadataReference.CreateFromFile(Path.Combine(assemblyPath!, "mscorlib.dll")),
        MetadataReference.CreateFromFile(Path.Combine(assemblyPath!, "System.dll")),
        MetadataReference.CreateFromFile(Path.Combine(assemblyPath!, "System.Core.dll")),
        MetadataReference.CreateFromFile(Path.Combine(assemblyPath!, "System.Runtime.dll")),
      });
      // コンパイルするコードで参照するdll
      references.AddRange(new [] {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
      });
    }

    // コンパイル情報
    var compilation = CSharpCompilation.Create(
      "HelloWorldCompiled.exe",
      options: new CSharpCompilationOptions(OutputKind.ConsoleApplication),
      syntaxTrees: new [] { tree },
      references: references
    );

    // コンパイル＆実行
    using (var stream = new MemoryStream())
    {
      // コンパイル
      var compileResult = compilation.Emit(stream);

      // コンパイル失敗時
      if (!compileResult.Success)
      {
        Console.WriteLine("===============ERROR===============");
        foreach(var diagnostic in compileResult.Diagnostics)
        {
          Console.WriteLine(diagnostic.ToString());
        }
        Console.WriteLine("===============ERROR===============");
      }

      // アセンブリコードの取得＆実行
      var assembly = Assembly.Load(stream.GetBuffer());
      assembly.EntryPoint?.Invoke(
        null,
        BindingFlags.NonPublic | BindingFlags.Static,
        null,
        new object[] { },
        null
      );
    }

    Console.Read();
  }
}
