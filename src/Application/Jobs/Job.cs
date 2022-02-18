using System.ComponentModel;
using System.Diagnostics;
using Hippo.Application.Common.Exceptions;

namespace Hippo.Application.Jobs;

public abstract class Job
{
    /// <summary>Gets an ID for this Job instance.</summary>
    /// <value>The identifier that is assigned by the system to this Job instance.</value>
    public readonly Guid Id = Guid.NewGuid();

    protected readonly CancellationToken cancellationToken = CancellationToken.None;

    protected readonly JobScheduler _jobScheduler = JobScheduler.Current;

    protected JobStatus _status = JobStatus.Created;

    public Job(Guid id)
    {
        Id = id;
    }

    public JobStatus Status
    {
        get => _status;
    }

    public bool IsWaitingToRun
    {
        get
        {
            switch (_status)
            {
                case JobStatus.Created:
                case JobStatus.WaitingToRun:
                    return !cancellationToken.IsCancellationRequested;
                default:
                    return false;
            }
        }
    }

    public bool IsRunning
    {
        get => _status == JobStatus.Running;
    }

    public bool IsStopped
    {
        get
        {
            switch (_status)
            {
                case JobStatus.Created:
                case JobStatus.WaitingToRun:
                    return cancellationToken.IsCancellationRequested;
                case JobStatus.Canceled:
                    return true;
                default:
                    return false;
            }
        }
    }

    public bool IsCompleted
    {
        get
        {
            switch (_status)
            {
                case JobStatus.Completed:
                case JobStatus.Canceled:
                case JobStatus.Unknown:
                    return true;
                default:
                    return false;
            }
        }
    }

    public bool IsCompletedSuccessfully
    {
        get => _status == JobStatus.Completed;
    }

    public Job() { }

    public Job(CancellationToken token)
    {
        cancellationToken = token;
    }

    public void Start()
    {
        _jobScheduler.Enqueue(this);
        _status = JobStatus.WaitingToRun;
    }

    public void Start(JobScheduler scheduler)
    {
        scheduler.Enqueue(this);
        _status = JobStatus.WaitingToRun;
    }

    /// <summary>
    /// Called by the <see cref="JobScheduler" /> when the <see cref="Job" /> is scheduled for execution.
    /// </summary>
    /// <exception cref="JobFailedException">when the <see cref="Job" /> fails to run.</exception>
    public abstract void Run();

    /// <summary>
    /// Waits for the <see cref="Job" /> to complete execution.
    /// </summary>
    /// <remarks>
    /// Wait is a synchronization method that causes the calling thread to wait until the <see cref="Job" /> has completed.
    /// It blocks the calling thread until the <see cref="Job" /> completes.
    ///
    /// If the <see cref="Job" /> has not begun execution, the calling thread will block until the <see cref="Job" /> starts running.
    /// </remarks>
    public void Wait()
    {
        // return early if the process has already finished
        if (IsCompleted)
        {
            return;
        }
        while (IsWaitingToRun || IsRunning)
        {
            Thread.Sleep(10);
        }
    }

    /// <summary>
    /// Stops the <see cref="Job" />, marking it as completed.
    /// </summary>
    public abstract void Stop();

    /// <summary>
    /// Reloads <see cref="Job" /> configuration.
    /// </summary>
    public abstract void Reload();
}
