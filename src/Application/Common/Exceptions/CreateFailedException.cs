namespace Hippo.Application.Common.Exceptions;

public class CreateFailedException : Exception
{
    public CreateFailedException()
        : base()
    {
    }

    public CreateFailedException(string message)
        : base(message)
    {
    }

    public CreateFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public CreateFailedException(string name, object key)
        : base($"Created to fail \"{name}\" ({key}).")
    {
    }
}
