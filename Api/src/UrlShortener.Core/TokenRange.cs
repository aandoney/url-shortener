namespace UrlShortener.Core;

public record TokenRange
{
    public TokenRange(long start, long end)
    {
        if (end < start)
        {
            throw new ArgumentException("End must be greater or equal to start");
        }
        
        Start = start;
        End = end;
    }
    
    public long Start { get; set; }
    public long End { get; set; }
}