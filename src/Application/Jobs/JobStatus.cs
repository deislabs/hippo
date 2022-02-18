namespace Hippo.Application.Jobs;

public enum JobStatus
{
    /// The job has been initialized but has not yet been scheduled.
    Created,
    /// The job has been scheduled for execution but has not yet begun executing.
    WaitingToRun,
    /// The job is running but has not yet completed.
    Running,
    /// The job completed execution successfully.
    Completed,
    /// The job acknowledged that it has been canceled.
    Canceled,
    /// The job's status is unknown due to an unhandled exception.
    Unknown,
}
