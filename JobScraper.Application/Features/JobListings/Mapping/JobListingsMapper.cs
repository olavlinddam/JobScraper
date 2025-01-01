using JobScraper.Contracts.Responses.JobListings;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.JobListings.Mapping;

public static class JobListingsMapper
{
    public static List<GetJobListingsResponse> MapToJobListingsResponses(List<JobListing> jobListings)
    {
        var jobListingsResponses = new List<GetJobListingsResponse>();
        foreach (var jobListing in jobListings)
        {
            var response = new GetJobListingsResponse(
                    Id: jobListing.Id,
                    Title: jobListing.Title,
                    Description: jobListing.Description,
                    CompanyName: jobListing.CompanyName,
                    ExpirationDate: jobListing.ExpirationDate,
                    PostedDate: jobListing.PostedDate,
                    Url: jobListing.Url,
                    CityName: jobListing.City.Name,
                    ZipCode: jobListing.City.Zip ?? null);
            
            jobListingsResponses.Add(response);
        }
        return jobListingsResponses;
    }
}