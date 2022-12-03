namespace MyThreadTest
{
  public static class BasicLongRunningOp
  {
    /// <summary>
    /// 重たい処理（引数あり）
    /// </summary>
    public static void ExecuteLongRunningOperation()
    {
      Thread.Sleep(10000);
      Console.WriteLine("Operation completed successfully");
    }

    /// <summary>
    /// 重たい処理（引数あり）
    /// </summary>
    public static void ExecuteLongRunningOperation(object? milliseconds) 
    {
      if (milliseconds is int msec)
      {
        Thread.Sleep(msec);
      }
      Console.WriteLine("Operation completed successfully (on Parameter)");
    }

  }
}