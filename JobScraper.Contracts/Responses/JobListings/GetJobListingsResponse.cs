using System.Reflection;

namespace JobScraper.Contracts.Responses.JobListings;

public record GetJobListingsResponse(
    int Id,
    string Title,
    string CompanyName,
    DateTime PostedDate,
    DateTime ExpirationDate,
    string Url,
    string Description,
    string CityName,
    int? ZipCode);