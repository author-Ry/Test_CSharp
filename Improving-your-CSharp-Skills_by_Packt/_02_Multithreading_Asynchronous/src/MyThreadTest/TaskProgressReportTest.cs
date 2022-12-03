namespace MyThreadTest
{
  public static class TaskProgressReportTest
  {
    public static void TaskProgressReportRun()
    {
     // 実際には処理しないので空
      string filePath = "";
      byte[] fileBytes = new byte[0];

      // 進捗確認用ハンドラ
      var progressHandler = new Progress<string>(value => {
        Console.WriteLine(value);
      });

      var progress = progressHandler as IProgress<string>;

      // キャンセルトークン
      CancellationTokenSource tokenSource = new CancellationTokenSource();
      CancellationToken token = tokenSource.Token;

      Task.Factory.StartNew(() => _SaveFileAsync(filePath, fileBytes, token, progress));

      // タスクのキャンセル
      tokenSource.Cancel();
    }

    /// <summary>
    /// タスク進捗確確認用メソッド : キャンセルトークン用メソッド
    /// </summary>
    private static Task<int> _SaveFileAsync(string path, byte[] fileBytes, CancellationToken cancellationToken, IProgress<string> progress)
    {
      if (cancellationToken.IsCancellationRequested)
      {
        progress.Report("Cancellation is called");
        Console.WriteLine("Cancellation is requested...");
        cancellationToken.ThrowIfCancellationRequested();
      }

      progress.Report("Saving File");

      // Do some file save operation
      // File.WriteAllBytes(path, fileBytes);

      progress.Report("File Saved");
      return Task.FromResult<int>(0);
    }
  }
}