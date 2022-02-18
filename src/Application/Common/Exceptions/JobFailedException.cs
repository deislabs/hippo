namespace Hippo.Application.Common.Exceptions;

public class JobFailedException : Exception
{
    public JobFailedException()
        : base($"Job failed.")
    {
    }

    public JobFailedException(string message)
        : base(message)
    {
    }

    // TODO: write unit tests using multiple reasons as input
    public JobFailedException(string[] reasons)
        : base("Job failed: " + string.Join(", ", reasons))
    {
    }

    public JobFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
