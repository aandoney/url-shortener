using UrlShortener.Core;

namespace UrlShortener.Api.Core.Tests;

public class TokenRangeScenarios
{
    [Fact]
    public void When_start_token_is_greater_than_end_token_then_throw_exception()
    {
        var act = () => new TokenRange(10, 5);
        
        act
            .ShouldThrow<ArgumentException>("End must be greater or equal to start");
    }
}