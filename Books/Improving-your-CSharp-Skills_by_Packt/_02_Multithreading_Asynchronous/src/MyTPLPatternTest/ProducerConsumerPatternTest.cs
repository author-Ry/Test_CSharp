using System.Collections.Concurrent;

namespace MyTPLPatternTest
{
  /// <summary>
  /// Producer/Consumer Pattern<br/>
  /// Task (producer1, producer2, producer3) -> Bloking Collection -> Task (consumer1, consumer2)
  /// </summary>
  public static class ProducerConsumerPatternTest
  {
    public static void ProducerConsumerPatternTestRun()
    {
      // コレクションの最大保持数
      int maxColl = 5;
      var blockingCollection = new BlockingCollection<int>(maxColl);
      var taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
      
      Console.WriteLine("Add Producer");

      // DBから画像読み込みを行うタスク
      Task producer = taskFactory.StartNew(() => {
        // 10回読み込み実行したら終了
        int counter = 0;
        while (counter < 10)
        {
          // コレクションの保持数が最大値に満たなければ追加
          if (blockingCollection.Count < maxColl)
          {
            int imageId = _ReadImageFromDB();
            blockingCollection.Add(imageId);
            counter++;
          }
        }
        // コレクションの終了
        blockingCollection.CompleteAdding();
      });

      Thread.Sleep(10000);
      Console.WriteLine("Add Counsumer");

      // 画像処理タスク
      Task consumer = taskFactory.StartNew(() => {
        // コレクションが終了してなければ実行
        while (!blockingCollection.IsCompleted)
        {
          try
          {
            // コレクションから画像データの取得（ここではID）
            int imageId = blockingCollection.Take();
            _ProcessImage(imageId);
          }
          catch (Exception ex)
          {
            // err log
            Console.WriteLine($"err: {ex.Message}");
          }
        }
      });
    }

    /// <summary>
    /// DBから画像ファイルの読み込み
    /// </summary>
    private static int _ReadImageFromDB()
    {
      Thread.Sleep(1000);
      Console.WriteLine("Image is read");
      return 1;
    }

    /// <summary>
    /// 画像処理
    /// </summary>
    private static void _ProcessImage(int imageId)
    {
      Thread.Sleep(1000);
      Console.WriteLine("Image is processed");
    }
  }
}