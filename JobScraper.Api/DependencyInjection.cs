using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace JobScraper.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddProblemDetails();

        AddLogging(services, configuration);

        return services;
    }

    private static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration) // Read configuration from appsettings.json
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning) // no logs lower than warning
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Information() // from the system in general
            .Enrich.FromLogContext()
            .WriteTo.Console(new CompactJsonFormatter()) // Structured logging to console
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                restrictedToMinimumLevel: LogEventLevel.Warning,
                formatter: new CompactJsonFormatter()
            )
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(dispose: true);
        });
        
        return services;
    }
}