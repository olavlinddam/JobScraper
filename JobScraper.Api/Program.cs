using JobScraper.Api;
using JobScraper.Application;
using JobScraper.Infrastructure;
using JobScraper.Infrastructure.Persistence;
using JobScraper.Infrastructure.StartUp;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Serilog.Settings.Configuration;

try
{
    Log.Information("Starting web host");
    var builder = WebApplication.CreateBuilder(args);
    {
        builder.Services.AddPresentation(builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddSwaggerGen();
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

        //app.UseHttpsRedirection();
        app.MapControllers();

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var con = context.Database.GetDbConnection().ConnectionString;
        Console.WriteLine($"DB_HOST: {Environment.GetEnvironmentVariable("DB_HOST")}");
        Console.WriteLine($"Selenium_Url: {Environment.GetEnvironmentVariable("SELENIUM_URL")}");
        Console.WriteLine(con);
        context.Database.Migrate();

        app.Run();
    }
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
}