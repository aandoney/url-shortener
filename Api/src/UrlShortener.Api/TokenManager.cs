using UrlShortener.Core;

public class TokenManager : IHostedService
{
    private readonly ITokenRangeApiClient _client;
    private readonly ILogger<TokenManager> _logger;
    private readonly string _machineIdentifier;
    private readonly TokenProvider _tokenProvider;

    public TokenManager(ITokenRangeApiClient client, ILogger<TokenManager> logger,
        TokenProvider tokenProvider)
    {
        _logger = logger;
        _tokenProvider = tokenProvider;
        _client = client;
        _machineIdentifier = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")
            ?? "unknown";
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting token manager");

        _tokenProvider.ReachingRangeLimit += async (sender, args) =>
        {
            await AssignNewRangeAsync(cancellationToken);
        };

        await AssignNewRangeAsync(cancellationToken);
    }

    private async Task AssignNewRangeAsync(CancellationToken cancellationToken)
    {
        var range = await _client.AssignRangeAsync(_machineIdentifier, cancellationToken);
        if (range is null)
        {
            throw new Exception("No tokens assigned");    
        }
        
        _tokenProvider.AssignRange(range);
        _logger.LogInformation("Assigned range: {Start}-{End}", range.Start, range.End);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping token manager");
        return Task.CompletedTask;
    }
}

public interface ITokenRangeApiClient
{
    Task<TokenRange?> AssignRangeAsync(string machineKey, CancellationToken cancellationToken);
}

public class TokenRangeApiClient : ITokenRangeApiClient
{
    private readonly HttpClient _httpClient;
    
    public TokenRangeApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("TokenRangeService");
    }
    
    public async Task<TokenRange?> AssignRangeAsync(string machineKey, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("/assign",
            new { Key = machineKey }, cancellationToken);
        
        if (response.IsSuccessStatusCode == false)
        {
            throw new Exception("Failed to assign new token range");
        }
        
        var range = await response.Content
            .ReadFromJsonAsync<TokenRange>(cancellationToken);
        
        return range;
    }
}