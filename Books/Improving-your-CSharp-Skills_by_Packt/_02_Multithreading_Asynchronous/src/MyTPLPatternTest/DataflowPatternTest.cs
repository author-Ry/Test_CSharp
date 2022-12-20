namespace MyTPLPatternTest
{
  /// <summary>
  /// Dataflow Pattern<br/>
  /// (Task1, Task2) -> Task3 -> (Task4, Task5)
  /// </summary>
  public static class DataflowPatternTest
  {
    public static void DataflowPatternTestRun()
    {
      // Task1, Task2（最初のタスク）
      Task<int> t1 = Task.Factory.StartNew(() => _TaskSample("Task1"));
      Task<int> t2 = Task.Factory.StartNew(() => _TaskSample("Task2"));
      
      // Task1 と Task2 が完了したら実行（Task3）
      Task<int> t3 = Task.Factory.ContinueWhenAll(
        new[] { t1, t2 },
        (tasks) => _TaskSample("Task3")
      );

      // Task3 が完了したら実行（Task4, Task5）
      Task<int> t4 = t3.ContinueWith((antecendent) => _TaskSample("Task4"));
      Task<int> t5 = t3.ContinueWith((antecendent) => _TaskSample("Task5"));
    }

    private static int _TaskSample(string taskName)
    {
      Thread.Sleep(1000);
      Console.WriteLine($"{taskName} is executed");
      return 1;
    }
  }
}