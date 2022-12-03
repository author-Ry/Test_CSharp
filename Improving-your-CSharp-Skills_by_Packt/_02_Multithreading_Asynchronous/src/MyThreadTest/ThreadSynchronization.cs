namespace MyThreadTest
{
  /// <summary>
  /// スレッドセーフではないシングルトンクラス
  /// </summary>
  public class NonThreadSafeLogger
  {
    private static NonThreadSafeLogger? _instance;

    private NonThreadSafeLogger() { }

    public NonThreadSafeLogger GetInstance()
    {
      _instance = (_instance is null ? new NonThreadSafeLogger() : _instance);
      return _instance;
    }
  }

  /// <summary>
  /// スレッドセーフなシングルトンクラス（lock）
  /// </summary>
  public class ThreadSafeLogger
  {
    static object _syncRoot = new object();
    static ThreadSafeLogger? _instance;

    private ThreadSafeLogger() { }

    public ThreadSafeLogger GetInstance()
    {
      if (_instance is null)
      {
        lock (_syncRoot)
        {
          if (_instance is null)
          {
            _instance = new ThreadSafeLogger();
          }
        }
      }
      return _instance;
    }
  }
}