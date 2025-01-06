using JobScraper.Contracts.Responses.JobListings;

namespace JobScraper.Contracts.Responses.Cities;

public record CityWithListingsResponse(
    int Id,
    string Name,
    string Country,
    int? Zip,
    List<JobListingsDto> JobListings);
    
public record JobListingsDto(
    int Id,
    string Title,
    string CompanyName,
    DateTime PostedDate,
    DateTime ExpirationDate,
    string Url,
    string Description);
