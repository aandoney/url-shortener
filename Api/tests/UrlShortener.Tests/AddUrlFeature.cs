using System.Net;
using System.Net.Http.Json;
using Shouldly;
using UrlShortener.Core.Urls.Add;

namespace UrlShortener.Tests;

public class AddUrlFeature : IClassFixture<ApiFixture>
{
    private readonly HttpClient _client;

    public AddUrlFeature(ApiFixture fixture)
    {
        _client = fixture.CreateClient();
    }
    
    [Fact]
    public async Task Given_Long_Url_Should_Return_Short_Url()
    {
        var response = await _client.PostAsJsonAsync<AddUrlRequest>("/api/urls",
            new AddUrlRequest(new Uri("https://google.com"), string.Empty));
        
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var addUrlResponse = await response.Content.ReadFromJsonAsync<AddUrlResponse>();
        
        addUrlResponse!.ShortUrl.ShouldNotBeNull();
    }
}