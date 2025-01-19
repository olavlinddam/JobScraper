using System.Collections;
using JobScraper.Api;
using JobScraper.Application;
using JobScraper.Application.Features.ClaudeIntegration;
using JobScraper.Infrastructure.ClaudeApi;
using JobScraper.Infrastructure.Persistence;
using JobScraper.Infrastructure.StartUp;
using Microsoft.EntityFrameworkCore;
using Serilog;

try
{
    Log.Information("Starting web host");
    var builder = WebApplication.CreateBuilder(args);
    {
        builder.Services.AddPresentation(builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpClient<IClaudeApiClient, ClaudeApiClient>();
    }

    var app = builder.Build();
    {
        app.UseRouting();
        app.UseSwagger();

        if (builder.Environment.IsDevelopment())
        {
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = "swagger";
            });
        }

        // app.UseHttpsRedirection();
        app.MapControllers();

        // Ensure migration
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        app.Run();
    }
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
}
