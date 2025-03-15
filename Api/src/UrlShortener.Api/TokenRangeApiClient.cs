using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using UrlShortener.Core;

public class TokenRangeApiClient : ITokenRangeApiClient
{
    private readonly HttpClient _httpClient;
    
    private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    
    public TokenRangeApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("TokenRangeService");
    }
    
    public async Task<TokenRange?> AssignRangeAsync(string machineKey, CancellationToken cancellationToken)
    {
        var response = await RetryPolicy.ExecuteAsync(() =>
            _httpClient.PostAsJsonAsync("/assign",
                new { Key = machineKey }, cancellationToken));
        
        if (response.IsSuccessStatusCode == false)
        {
            throw new Exception("Failed to assign new token range");
        }
        
        var range = await response.Content
            .ReadFromJsonAsync<TokenRange>(cancellationToken);
        
        return range;
    }
}