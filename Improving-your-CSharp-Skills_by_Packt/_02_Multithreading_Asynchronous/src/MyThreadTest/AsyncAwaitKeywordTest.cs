namespace MyThreadTest
{
  public static class AsyncAwaitKeywordTest
  {
    public static void AsyncAwaitKeywordTestRun()
    {
      var t = _ExecuteLongRunningOpAysnc(10000);
      Console.WriteLine("Called ExecuteLongRunningOperationAsync method, now waiting for it to complete");
      t.Wait();
    }

    /// <summary>
    /// 単純なループのタスク実行
    /// </summary>
    private static async Task<int> _ExecuteLongRunningOpAysnc(int millis)
    {
      Task t = Task.Factory.StartNew(() => _RunLoopAsync(millis));
      await t;
      Console.WriteLine("Executed RunLoopAsync method");
      return 0;
    }

    /// <summary>
    /// 単純なループ
    /// </summary>
    private static void _RunLoopAsync(int millis)
    {
      Console.WriteLine("Inside RunLoopAsync method");
      for (int i = 0; i < millis; i++)
      {
        Console.WriteLine($"Counter = {i}");
      }
      Console.WriteLine("Exiting RunLoopAsync method");
    }
  }
}