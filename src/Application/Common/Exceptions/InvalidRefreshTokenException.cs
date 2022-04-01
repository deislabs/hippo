namespace Hippo.Application.Common.Exceptions;

public class InvalidRefreshTokenException : Exception
{
    public InvalidRefreshTokenException()
        : base($"Invalid refresh token.")
    {
    }

    public InvalidRefreshTokenException(string message)
        : base(message)
    {
    }

    // TODO: write unit tests using multiple reasons as input
    public InvalidRefreshTokenException(string[] reasons)
        : base("Invalid refresh token: " + string.Join(", ", reasons))
    {
    }

    public InvalidRefreshTokenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
