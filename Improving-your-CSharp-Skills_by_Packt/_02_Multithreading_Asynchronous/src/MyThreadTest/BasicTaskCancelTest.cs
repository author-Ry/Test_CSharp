namespace MyThreadTest
{
  public static class BasicTaskCancelTest
  {
    public static void BasicTaskCancelRun()
    {
      // 実際には処理しないので空
      string filePath = "";
      byte[] fileBytes = new byte[0];

      CancellationTokenSource tokenSource = new CancellationTokenSource();
      CancellationToken token = tokenSource.Token;
      Task.Factory.StartNew(() => _SaveFileAsync(filePath, fileBytes, token));

      // タスクのキャンセル
      tokenSource.Cancel();
    }

    /// <summary>
    /// キャンセルトークン用メソッド
    /// </summary>
    private static Task<int> _SaveFileAsync(string path, byte[] fileBytes, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
        Console.WriteLine("Cancellation is requested...");
        cancellationToken.ThrowIfCancellationRequested();
      }

      // Do some file save operation
      // File.WriteAllBytes(path, fileBytes);

      return Task.FromResult<int>(0);
    }
  }
}