namespace MyTPLPatternTest
{
  /// <summary>
  /// Pipeline Pattern<br/>
  /// Task1 -> Task2 -> Task3
  /// </summary>
  public static class PipelinePatternTest
  {
    public static void PipelinePatternTestRun()
    {
      // Task1（ユーザ作成タスク）
      Task<int> t1 = Task.Factory.StartNew(() => _CreateUser());

      // Task1 の後に実行（ワークフロー実行タスク）
      var t2 = t1.ContinueWith((antecedent) => _InitiateWorkflow(antecedent.Result));

      // Task2 の後に実行（メール送信タスク）
      var t3 = t2.ContinueWith((antecedent) => _SendEmail(antecedent.Result));
    }

    private static int _CreateUser()
    {
      Thread.Sleep(1000);
      Console.WriteLine("User created");
      return 1;
    }

    private static int _InitiateWorkflow(int userId)
    {
      Thread.Sleep(1000);
      Console.WriteLine("Workflow initiates");
      return userId;
    }

    private static int _SendEmail(int userId)
    {
      Thread.Sleep(1000);
      Console.WriteLine("Email sent");
      return userId;
    }
  }
}