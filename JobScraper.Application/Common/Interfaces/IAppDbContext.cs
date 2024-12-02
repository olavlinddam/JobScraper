using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScraper.Application.Common.Interfaces;

public interface IAppDbContext
{
    public DbSet<Website> Websites { get; set; }
    public DbSet<ScrapingPolicy> ScrapingPolicies { get; set; }
}