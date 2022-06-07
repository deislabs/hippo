using Hippo.Application.Common.Exceptions;

namespace Hippo.Application.Jobs;

public abstract class Job
{
    /// <summary>Gets an ID for this Job instance.</summary>
    /// <value>The identifier that is assigned by the system to this Job instance.</value>
    public readonly Guid Id = Guid.NewGuid();
    public JobStatus Status { get; set; }

    protected readonly CancellationToken cancellationToken = CancellationToken.None;

    public Job(Guid id)
    {
        Id = id;
    }

    public Job(Guid id, JobStatus status)
    {
        Id = id;
        Status = status;
    }

    public Job() { }

    public Job(CancellationToken token)
    {
        cancellationToken = token;
    }
}
