using Testcontainers.PostgreSql;

namespace JobScraper.IntegrationTests;

public abstract class IntegrationTestBase : IAsyncDisposable
{
    private readonly PostgreSqlContainer _dbContainer;
    protected string ConnectionString;

    protected IntegrationTestBase()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithPassword("integration_test_pwd")
            .WithUsername("integration_test_user")
            .WithDatabase("integration_test_db")
            .Build();
    }

    protected async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        ConnectionString = _dbContainer.GetConnectionString();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
