namespace Hippo.Application.Jobs;

public abstract class Job
{
    /// <summary>Gets an ID for this Job instance.</summary>
    /// <value>The identifier that is assigned by the system to this Job instance.</value>
    public readonly Guid Id = Guid.NewGuid();

    protected readonly CancellationToken cancellationToken = CancellationToken.None;

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
}
