namespace MyThreadTest
{
  public static class ThreadLifetime
  {
    // 変数にvolatileをつけることで、生存期間の長いスレッドを正しく処理できる
    // Thread:A-1 | 最適化により変数削除 | Thread:A-1(続き) ...
    // 適切に処理しない場合、変数が不定となりループ処理の場合無限ループとなる
    static volatile bool _isActive = true;

    /// <summary>
    /// volatileを用いたスレッドの有効期間
    /// </summary>
    public static void ExecuteLongRunningOperation()
    {
      while (_isActive)
      {
        // some operation
        Console.WriteLine("Operation completed successfully");
      }
    }

  }
}