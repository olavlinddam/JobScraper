using ReverseMarkdown.Converters;

namespace JobScraper.Infrastructure.Persistence.Configurations;

public class DatabaseSettings()
{
    public required string Host { get; set; } 
    public required string Name { get; set; }
    public required string User { get; set; }
    public required string Password { get; set; }

    public string ConnectionString => $"Host={Host};Database={Name};Username={User};Password={Password}";    
}
