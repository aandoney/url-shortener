namespace UrlShortener.TokenRangesService;

public class FailedToAssignRangeException : Exception
{
    public FailedToAssignRangeException(string message) : base(message)
    {
    }
}