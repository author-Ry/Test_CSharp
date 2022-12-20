using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Filters;

namespace MyBenchmarkDotNetTest
{
  [MemoryDiagnoser]
//  [Config(typeof(Config))]
  public class MyFibonacci
  {
    /// <summary>
    /// Benchmark時の任意の設定用クラス
    /// </summary>
    private class Config : ManualConfig
    {
      public Config()
      {
        // ここでは"Recursive"を含むメソッドのみを実行するフィルタ
        AddFilter(new DisjunctionFilter(
          new NameFilter(name => name.Contains("Recursive"))
        ));
      }
    }

    [Params(10, 20, 30)]
    public int Len { get; set; }

    [Benchmark]
    public void CreateFibonacci()
    {
      int a = 0, b = 1, c = 0;
      Console.WriteLine($"{a} {b}");

      for (int i = 2; i < Len; i++)
      {
        c = a + b;
        Console.WriteLine($" {c}");
        a = b;
        b = c;
      }
    }

    // 再帰的にフィボナッチ数列を生成
    [Benchmark]
    public void CreateFibonacciRecursive()
    {
      _FibonacciRecursive(0, 1, 1, Len);
    }

    private void _FibonacciRecursive(int a, int b, int counter, int len)
    {
      if (counter <= len)
      {
        Console.WriteLine($"{a} ");
        _FibonacciRecursive(b, a + b, counter + 1, len);
      }
    }
  }
}
