using MyThreadTest;
using MyTaskParallelLibraryTest;
public static class Program
{
  public static void Main()
  {
    // 別スレッドで実行
    new Thread(new ThreadStart(BasicLongRunningOp.ExecuteLongRunningOperation)).Start();

    // 上記引数指定ver
    new Thread(new ParameterizedThreadStart(BasicLongRunningOp.ExecuteLongRunningOperation)).Start(10000);

    // serviceを用いて呼び出し
    IService service = new EmailService("Email");
    new Thread(new ParameterizedThreadStart(EmailService.RunBackgroundService)).Start(service);

    // volatileを用いた有効期間有りの別スレッド起動
    // 実行方法は上記と同じ
    // src/MyThreadTest/ThreadLifetime.cs

    // スレッドプールのタスクキュー（FIFO）への追加
    ThreadPool.QueueUserWorkItem(BasicLongRunningOp.ExecuteLongRunningOperation, 20000);

    // lockパターンによるスレッドセーフなシングルトンクラス
    // src/MyThreadTest/ThreadSynchronization.cs

    // Monitorを用いたマルチスレッド処理
    // src/MyThreadTest/Monitor.cs
    // MonitorTest.MonitorTestRun();

    // TPL (Task Parallel Library) を用いた非同期処理
    // TAP (Task-based Asynchronous Pattern) のベストプラクティス例： MethodName + Async
    Task t = Task.Run(() => BasicLongRunningOp.ExecuteLongRunningOperation(30000));
    t.Wait();

    // タスクのキャンセル
    BasicTaskCancelTest.BasicTaskCancelRun();

    // タスクの進捗状況確認
    TaskProgressReportTest.TaskProgressReportRun();

    // async/await キーワードを用いたTAP実装
    AsyncAwaitKeywordTest.AsyncAwaitKeywordTestRun();

    // タスクを細かく制御するTAP実装
    TaskCompletionSourceTest.TaskCompletionSourceRun();

    Console.Read();
  }

}