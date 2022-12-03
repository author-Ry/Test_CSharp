using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Running;
using MyBenchmarkDotNetTest;

public class Program
{
  public static void Main()
  {
    // ベンチマーク実行
    BenchmarkRunner.Run<MyFibonacci>();

    /*
    // configを設定して実行
    var config = ManualConfig.Create(DefaultConfig.Instance);
    config.AddFilter(new DisjunctionFilter(new NameFilter(
      name => name.Contains("Recursive")
    )));
    // 引数で渡して適用
    BenchmarkRunner.Run<MyFibonacci>(config);
    */
 
    Console.Read();
  }
}
