namespace Hippo.Application.Common.Exceptions;

public class LoginFailedException : Exception
{
    public LoginFailedException()
        : base($"Login failed.")
    {
    }

    public LoginFailedException(string message)
        : base(message)
    {
    }

    // TODO: write unit tests using multiple reasons as input
    public LoginFailedException(string[] reasons)
        : base("Login failed: " + string.Join(", ", reasons))
    {
    }

    public LoginFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
