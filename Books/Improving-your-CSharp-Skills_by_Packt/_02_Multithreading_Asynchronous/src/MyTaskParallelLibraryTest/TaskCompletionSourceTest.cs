using MyThreadTest;

namespace MyTaskParallelLibraryTest
{
  public static class TaskCompletionSourceTest
  {
    public static void TaskCompletionSourceRun()
    {
      var t = _ExecuteTask();
      t.Wait();
    }

    private static Task<int> _ExecuteTask()
    {
      var tcs = new TaskCompletionSource<int>();
      Task.Factory.StartNew(() =>
      {
        try
        {
          BasicLongRunningOp.ExecuteLongRunningOperation(10000);
          tcs.SetResult(1);
        } catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      });

      return tcs.Task;
    }
  }
}