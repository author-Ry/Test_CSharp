namespace MyThreadTest
{
  public interface IService
  {
    string Name { get; set; }
    void Execute();
  }

  public class EmailService : IService
  {
    public string Name { get; set; }
    public void Execute()
    {
      Console.WriteLine($"Service Execute: {Name}");
      // throw new NotImplementedException();
    }

    public EmailService(string name)
    {
      Name = name;
    }

    /// <summary>
    /// serviceを用いた別スレッドでの実行
    /// </summary>
    public static void RunBackgroundService(object? service)
    {
      if (service is IService s)
      {
        // スレッドの優先度付け
        Thread.CurrentThread.Priority = ThreadPriority.Highest;
        s.Execute(); // Long running task
      }
    }
  }

}