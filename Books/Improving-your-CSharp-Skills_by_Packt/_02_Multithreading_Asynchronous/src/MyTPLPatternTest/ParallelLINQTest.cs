namespace MyTPLPatternTest
{
  /// <summary>
  /// LINQでの並列実行
  /// </summary>
  public static class ParallelLINQTest
  {
    public static void ParallelLINQTestRun()
    {
      List<int> users = _GetUsers();

      users.AsParallel().Select(user => {
        Console.WriteLine($"(LINQ.AsParallel) userId: {user}");
        _UserProcess();
        Console.WriteLine($"(LINQ.AsParallel) userId: {user} processed");
        return user;
      }).ToList();
    }

    private static List<int> _GetUsers()
    {
      return new() { 1, 2, 3, 4, 5 };
    }

    private static void _UserProcess()
      => Thread.Sleep(1000);
  }
}