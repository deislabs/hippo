namespace Hippo.Application.Jobs;

/// <summary>
/// Represents an object that handles the low-level work of queuing jobs onto a scheduler.
/// </summary>
public abstract class JobScheduler
{
    /// <summary>
    /// Gets the default JobScheduler that is provided by Hippo.
    /// </summary>
    public static JobScheduler Default
    {
        get => new ThreadedJobScheduler();
    }

    /// <summary>
    /// Gets the current JobScheduler associated with the currently exeucting thread.
    /// </summary>
    /// <remarks>
    /// When not called from within a Job, Current will return the Default scheduler.
    /// </remarks>
    public static JobScheduler Current
    {
        get => _current;
    }

    private static readonly JobScheduler _current = Default;

    /// <summary>
    /// Initializes the JobScheduler.
    /// </summary>
    protected JobScheduler() { }

    /// <summary>
    /// Queues a Job to the scheduler.
    /// </summary>
    /// <remarks>
    /// A class derived from JobScheduler implements this method to accept jobs being scheduled on the scheduler.
    ///
    /// A typical implementation would store the job in an internal data structure, which would be serviced by a process that would execute those jobs at some time in the future.
    /// </remarks>
    protected internal abstract void Enqueue(Job job);

    /// <summary>
    /// Generates an enumerable of Job instances currently queued to the scheduler waiting to be executed.
    /// </summary>
    protected internal abstract IEnumerable<Job> GetScheduledJobs();

    /// <summary>
    /// Generates an enumerable of Job instances currently executing on the scheduler.
    /// </summary>
    protected internal abstract IEnumerable<Job> GetRunningJobs();
}
