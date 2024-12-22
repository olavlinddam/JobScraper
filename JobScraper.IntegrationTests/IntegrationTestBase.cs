using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using JobScraper.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace JobScraper.IntegrationTests;

public abstract class IntegrationTestBase : IAsyncDisposable
{
    private readonly PostgreSqlContainer _dbContainer;
    private readonly IContainer _seleniumContainer;
    protected AppDbContext DbContext;
    protected string ConnectionString;
    protected string SeleniumUrl;

    protected IntegrationTestBase()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithPassword("integration_test_pwd")
            .WithUsername("integration_test_user")
            .WithDatabase("integration_test_db")
            .Build();

        _seleniumContainer = new ContainerBuilder()
            .WithImage("selenium/standalone-chrome:latest")
            .WithPortBinding(4444, 4444)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(4444)
                .UntilHttpRequestIsSucceeded(request =>
                    request.ForPath("/wd/hub/status")
                        .ForPort(4444)))
            .Build();
    }

    protected async Task InitializeAsync()
    {
        Console.WriteLine("Starting containers..");

        try
        {
            Console.WriteLine("Starting database container...");
            await _dbContainer.StartAsync();
            ConnectionString = _dbContainer.GetConnectionString();
            Console.WriteLine($"Database ready at: {ConnectionString}");

            Console.WriteLine("Starting Selenium container...");
            await _seleniumContainer.StartAsync();
            SeleniumUrl = "http://localhost:4444/wd/hub";
            Console.WriteLine($"Selenium should be ready at: {SeleniumUrl}");

            // Test Selenium connection
            using var client = new HttpClient();
            var response = await client.GetAsync("http://localhost:4444/wd/hub/status");
            Console.WriteLine($"Selenium container status response: {response.StatusCode}");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Selenium response content: {content}");

            Console.WriteLine("Setting up database context...");
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(ConnectionString)
                .Options;
            DbContext = new AppDbContext(options);

            Console.WriteLine("Running database migrations...");
            await DbContext.Database.MigrateAsync();
            Console.WriteLine("Initialization complete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Initialization failed: {ex.GetType().Name}");
            Console.WriteLine($"Error message: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _dbContainer.DisposeAsync();
        await _seleniumContainer.DisposeAsync();
    }
}