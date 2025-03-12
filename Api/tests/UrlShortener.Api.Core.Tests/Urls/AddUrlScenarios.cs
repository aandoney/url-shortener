using Microsoft.Extensions.Time.Testing;
using UrlShortener.Api.Core.Tests.TestDoubles;
using UrlShortener.Core;
using UrlShortener.Core.Urls.Add;

namespace UrlShortener.Api.Core.Tests.Urls;

public class AddUrlScenarios
{
    private readonly AddUrlHandler _handler;
    private readonly InMemoryDataStore _urlDataStore = new ();
    private readonly FakeTimeProvider _timeProvider;

    public AddUrlScenarios()
    {
        var tokenProvider = new TokenProvider();
        tokenProvider.AssignRange(1, 5);
        var shortUrlGenerator = new ShortUrlGenerator(tokenProvider);
        _timeProvider = new FakeTimeProvider();
        _handler = new AddUrlHandler(shortUrlGenerator, _urlDataStore, _timeProvider);
    }
    
    [Fact]
    public async Task Should_return_shortened_url()
    {
        var request = CreateAddUrlRequest();
        
        var response = await _handler.HandleAsync(request, default);
            
        response.Succeeded.ShouldBeTrue();
        response.Value!.ShortUrl.ShouldNotBeEmpty();
        response.Value!.ShortUrl.ShouldBe("1");
    }

    [Fact]
    public async Task Should_save_short_url()
    {
        var request = CreateAddUrlRequest();
        
        var response = await _handler.HandleAsync(request, default);

        response.Succeeded.ShouldBeTrue();
        _urlDataStore.ShouldContainKey(response.Value!.ShortUrl);
    }
    
    [Fact]
    public async Task Should_save_short_url_with_created_by_and_and_created_on()
    {
        var request = CreateAddUrlRequest();
        
        var response = await _handler.HandleAsync(request, default);
        
        response.Succeeded.ShouldBeTrue();
        _urlDataStore.ShouldContainKey(response.Value!.ShortUrl);
        _urlDataStore[response.Value!.ShortUrl].CreatedBy.ShouldBe(request.CreatedBy);
        _urlDataStore[response.Value!.ShortUrl].CreatedOn.ShouldBe(_timeProvider.GetUtcNow());
    }

    [Fact]
    public async Task Should_return_error_if_created_by_is_empty()
    {
        var request = CreateAddUrlRequest(createdBy: string.Empty);
        
        var response = await _handler.HandleAsync(request, default);

        response.Succeeded.ShouldBe(false);
        response.Error.Code.ShouldBe("missing_value");
    }

    private static AddUrlRequest CreateAddUrlRequest(string createdBy = "aandoney@test.me")
    {
        return new AddUrlRequest(new Uri("https://google.com"), createdBy);
    }
}