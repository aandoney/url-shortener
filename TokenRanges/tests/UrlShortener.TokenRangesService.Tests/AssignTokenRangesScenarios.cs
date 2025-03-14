using System.Collections.Concurrent;
using System.Net.Http.Json;
using Shouldly;

namespace UrlShortener.TokenRangesService.Tests;

public class AssignTokenRangesScenarios : IClassFixture<Fixture>
{
    private readonly HttpClient _client;
    
    public AssignTokenRangesScenarios(Fixture fixture)
    {
        _client = fixture.CreateClient();
    }
    
    [Fact]
    public async Task Should_return_range_when_requested()
    {
        var requestResponse = await _client
            .PostAsJsonAsync("/assign", new AssignTokenRangeRequest("tests"));
        
        var tokenRange = await requestResponse
            .Content.ReadFromJsonAsync<TokenRangeResponse>();

        requestResponse.IsSuccessStatusCode.ShouldBeTrue();
        tokenRange.Start.ShouldBeGreaterThan(0);
        tokenRange.End.ShouldBeGreaterThan(tokenRange.Start);
    }
    
    [Fact]
    public async Task Should_not_repeat_range_when_requested()
    {
        var requestResponse1 = await _client
            .PostAsJsonAsync("/assign", new AssignTokenRangeRequest("tests"));
        var requestResponse2 = await _client
            .PostAsJsonAsync("/assign", new AssignTokenRangeRequest("tests"));
        
        requestResponse1.IsSuccessStatusCode.ShouldBeTrue();
        requestResponse2.IsSuccessStatusCode.ShouldBeTrue();
        var tokenRange1 = await requestResponse1
            .Content.ReadFromJsonAsync<TokenRangeResponse>();
        var tokenRange2 = await requestResponse2
            .Content.ReadFromJsonAsync<TokenRangeResponse>();
        
        tokenRange2.Start.ShouldBeGreaterThan(tokenRange1.End);
    }

    [Fact]
    public async Task Should_not_repeat_range_on_multiple_requests()
    {
        ConcurrentBag<TokenRangeResponse> ranges = [];
        await Parallel.ForEachAsync(Enumerable.Range(1, 100), async (number, cancellationToken) =>
        {
            var response = await _client.PostAsJsonAsync("/assign",
                new AssignTokenRangeRequest(number.ToString()), cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var range = await response.Content
                    .ReadFromJsonAsync<TokenRangeResponse>(cancellationToken);
                ranges.Add(range!);
            }
        });
        
        ranges.Select(x => x.Start).ShouldBeUnique();
        ranges.Select(x => x.End).ShouldBeUnique();
    }
}