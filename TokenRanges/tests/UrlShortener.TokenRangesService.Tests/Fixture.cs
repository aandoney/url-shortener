using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Testcontainers.PostgreSql;

namespace UrlShortener.TokenRangesService.Tests;

public class Fixture : WebApplicationFactory<ITokenRangesAssemblyMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private string ConnectionString => _postgreSqlContainer.GetConnectionString();

    public Fixture()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        
        Environment.SetEnvironmentVariable("Postgres__ConnectionString", ConnectionString);
        
        await InitializeSqlTable();
    }

    private async Task InitializeSqlTable()
    {
        var tableSql = await File.ReadAllTextAsync("Table.sql");

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(tableSql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
    }
}