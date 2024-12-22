using JobScraper.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace JobScraper.IntegrationTests;
public abstract class IntegrationTestBase : IAsyncDisposable
{
    private readonly PostgreSqlContainer _dbContainer;
    protected AppDbContext DbContext;
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
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;
            
        DbContext = new AppDbContext(options);
        
        await DbContext.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }
}
