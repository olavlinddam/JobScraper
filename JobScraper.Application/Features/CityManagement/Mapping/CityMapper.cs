using JobScraper.Contracts.Responses.Cities;
using ErrorOr;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.CityManagement.Mapping;

public static class CityMapper
{
    public static List<CityWithListingsResponse> MapToCitiesWithListings(List<City> cities)
    {
        return cities.Select(city => new CityWithListingsResponse
        (
            city.Id,
            city.Name,
            city.Country,
            city.Zip,
            city.JobListings.Select(listing => new JobListingsDto
            (
                listing.Id,
                listing.Title,
                listing.CompanyName,
                listing.PostedDate,
                listing.ExpirationDate,
                listing.Url,
                listing.Description
            )).ToList()
            )
        ).ToList();
    }
}