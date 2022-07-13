namespace Hippo.Application.Common.Exceptions;

public class AccountException : Exception
{
    public AccountException()
        : base($"A failure occurred with your account.")
    {
    }

    public AccountException(string message)
        : base(message)
    {
    }

    // TODO: write unit tests using multiple reasons as input
    public AccountException(string[] reasons)
        : base(string.Join(", ", reasons))
    {
    }

    public AccountException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
