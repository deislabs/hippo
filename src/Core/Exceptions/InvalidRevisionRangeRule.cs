namespace Hippo.Core.Exceptions;

public class InvalidRevisionRangeRuleException : Exception
{
    public InvalidRevisionRangeRuleException(string rule)
        : base($"Range rule \"{rule}\" is invalid.")
    {
    }
}
