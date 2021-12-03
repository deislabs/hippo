namespace Hippo.Infrastructure.Exceptions;

public class InvalidDatabaseDriverException : Exception
{
    public InvalidDatabaseDriverException(string driver)
        : base($"Database driver \"{driver}\" is invalid.")
    {
    }
}
