using System.Collections;

namespace MyThreadTest
{
  public class BasicJob
  {
    int _jobDone;
    object _lock = new Object();

    public void IncrementJobCounter(int number)
    {
      // 排他ロック取得
      Monitor.Enter(_lock);
      _jobDone += number;
      // 排他ロック解放
      Monitor.Exit(_lock);
    }
  }
  
  /// <summary>
  /// Monitorを用いたマルチスレッド実行例
  /// </summary>
  public class MonitorTest
  {
    public static void MonitorTestRun()
    {
      // Job実行処理スレッドの開始
      Thread jobThread = new Thread(new ThreadStart(ExecuteJobExecutor));
      jobThread.Start();

      // Job追加スレッド
      Thread addJobThread1 = new Thread(new ThreadStart(ExecuteThread1));
      Thread addJobThread2 = new Thread(new ThreadStart(ExecuteThread2));
      Thread addJobThread3 = new Thread(new ThreadStart(ExecuteThread3));
      addJobThread1.Start();
      addJobThread2.Start();
      addJobThread3.Start();
    }

    // job実行処理スレッド
    private static void ExecuteJobExecutor()
    {
      JobExecutor.Instance.CheckAndExecuteJobBatch();
    }

    // スレッド1 (AddJob)
    private static void ExecuteThread1()
    {
      Thread.Sleep(5000);
      List<Job> jobs = new List<Job> {
        new Job() { JobID = 11, JobName = "Thread 1 Job 1" },
        new Job() { JobID = 12, JobName = "Thread 1 Job 2" },
        new Job() { JobID = 13, JobName = "Thread 1 Job 3" },
      };
      JobExecutor.Instance.AddJobItems(jobs);
    }

    // スレッド2 (AddJob)
    private static void ExecuteThread2()
    {
      Thread.Sleep(5000);
      List<Job> jobs = new List<Job> {
        new Job() { JobID = 21, JobName = "Thread 2 Job 1" },
        new Job() { JobID = 22, JobName = "Thread 2 Job 2" },
        new Job() { JobID = 23, JobName = "Thread 2 Job 3" },
      };
      JobExecutor.Instance.AddJobItems(jobs);
    }

    // スレッド3 (AddJob)
    private static void ExecuteThread3()
    {
      Thread.Sleep(5000);
      List<Job> jobs = new List<Job> {
        new Job() { JobID = 31, JobName = "Thread 3 Job 1" },
        new Job() { JobID = 32, JobName = "Thread 3 Job 2" },
        new Job() { JobID = 33, JobName = "Thread 3 Job 3" },
      };
      JobExecutor.Instance.AddJobItems(jobs);
    }
  }

  /// <summary>
  /// jobの情報
  /// </summary>
  public class Job
  {
    public int JobID { get; set; }
    public string? JobName { get; set; }

    // jobの処理
    public void DoSomething()
    {
      Console.WriteLine($"Executed job - ID: {JobID}, Name: {JobName}");
    }
  }

  /// <summary>
  /// jobの実行担当（シングルトン）
  /// </summary>
  public class JobExecutor
  {
    const int _waitTimeInMillis = 10 * 60 * 1000;
    private ArrayList _jobs = new ArrayList();

    // シングルトン用
    private static JobExecutor? _instance = null;
    private static object _syncRoot = new object();

    // 自身のステータス
    private Boolean IsIdle { get; set; }
    private Boolean IsAlive { get; set; }

    public static JobExecutor Instance
    {
      get {
        lock (_syncRoot)
        {
          if(_instance is null)
          {
            _instance = new JobExecutor();
          }
        }
        return _instance;
      }
    }

    private JobExecutor()
    {
      IsIdle = true;
      IsAlive = true;
    }

    public void AddJobItems(List<Job> jobList)
    {
      lock (_jobs)
      {
        _jobs.AddRange(jobList);
        Monitor.PulseAll(_jobs);
      }
    }

    public void CheckAndExecuteJobBatch()
    {
      lock (_jobs)
      {
        while (IsAlive)
        {
          // job数が0
          if (_jobs.Count <= 0)
          {
            IsIdle = true;
            Console.WriteLine("Now waiting for new jobs");

            // lock解放、(Monitorのパルス or waitTimeの時間分) 待機
            Monitor.Wait(_jobs, _waitTimeInMillis);
          }
          else
          {
            IsIdle = false;

            // job実行
            ExecuteJob();
          }
        }
      }
    }

    private void ExecuteJob()
    {
      for (int i = 0; i < _jobs.Count; i++)
      {
        Job? job = (Job?)_jobs[i];
        if (job is Job)
        {
          job.DoSomething();
        }
        _jobs.Remove(job);
        i--;
      }
    }
  }
}