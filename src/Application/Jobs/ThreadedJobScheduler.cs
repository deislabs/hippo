using System.Collections.Concurrent;

namespace Hippo.Application.Jobs;

public class ThreadedJobScheduler : JobScheduler
{
    private readonly BlockingCollection<Job> _jobQueue = new BlockingCollection<Job>();

    private List<Job> _runningJobs = new List<Job>();

    public ThreadedJobScheduler()
    {
        var thread = new Thread(new ThreadStart(OnStart));
        thread.IsBackground = true;
        thread.Start();
    }

    protected internal override void Enqueue(Job job)
    {
        _jobQueue.Add(job);
    }

    protected internal override IEnumerable<Job> GetScheduledJobs()
    {
        return _jobQueue.AsEnumerable();
    }

    protected internal override IEnumerable<Job> GetRunningJobs()
    {
        lock (_runningJobs) return _runningJobs;
    }

    private void OnStart()
    {
        foreach (var job in _jobQueue.GetConsumingEnumerable(CancellationToken.None))
        {
            job.Run();
            lock (_runningJobs) _runningJobs.Add(job);

            // wait for the job's status to enter the running state
            Thread.Sleep(10);

            var thread = new Thread(new ThreadStart(() =>
            {
                job.Wait();
                lock (_runningJobs) _runningJobs.Remove(job);
            }));
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
