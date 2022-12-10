namespace MyTPLPatternTest
{
  /// <summary>
  /// 並列ループ
  /// </summary>
  public static class ParallelForEachTest
  {
    public static void ParallelForEachTestRun()
    {
      List<int> users = _GetUsers();

      // ここでは各ユーザ処理に1秒かかるので5秒かかる。かつList順に実行する
      users.ForEach(user => {
        Console.WriteLine($"(for) userId: {user}");
        _UserProcess();
        Console.WriteLine($"(for) userId: {user} processed");
      });

      // 各ユーザ処理を並列に実行する
      Parallel.ForEach(users, (user) => {
        Console.WriteLine($"(Parallel.ForEach) userId: {user}");
        _UserProcess();
        Console.WriteLine($"(Parallel.ForEach) userId: {user} processed");
      });
    }

    private static List<int> _GetUsers()
    {
      return new() { 1, 2, 3, 4, 5 };
    }

    private static void _UserProcess()
      => Thread.Sleep(1000);
  }
}