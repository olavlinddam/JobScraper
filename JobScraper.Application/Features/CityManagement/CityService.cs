using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Domain.Entities;
using ErrorOr;
using JobScraper.Application.Features.CityManagement.Mapping;
using JobScraper.Contracts.Responses.Cities;
using Microsoft.Extensions.Logging;

namespace JobScraper.Application.Features.CityManagement;

public class CityService
{
    private readonly ILogger<CityService> _logger;
    private readonly ICityRepository _cityRepository;

    public CityService(ILogger<CityService> logger, ICityRepository cityRepository)
    {
        _logger = logger;
        _cityRepository = cityRepository;
    }

    public async Task<ErrorOr<List<City>>> GetAllCities(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting all cities from database");
            var cities = await _cityRepository.GetAll(cancellationToken);

            if (cities.Count != 0)
                return cities;

            _logger.LogError("No cities found in database");
            
            return Error.NotFound(
                code: "City.NotFound",
                description: "No cities found");
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occured while fetching cities: {e}", e);
            throw;
        }
    }
    public async Task<ErrorOr<List<CityWithListingsResponse>>> GetAllCitiesWithListings(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting all cities with listings from database");
            var cities = await _cityRepository.GetAllWithListings(cancellationToken);

            if (cities.Count != 0)
                return CityMapper.MapToCitiesWithListings(cities);

            _logger.LogError("No cities found in database");
            
            return Error.NotFound(
                code: "City.NotFound",
                description: "No cities found");
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occured while fetching cities: {e}", e);
            throw;
        }
    }
}