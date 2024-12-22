using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScraper.IntegrationTests;

[TestClass]
public class DatabaseSetupTests : IntegrationTestBase
{
    [TestMethod]
    public async Task Database_ShouldBeAccessible()
    {
        // Arrange
        var url = "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch";
        var searchTerm = "Systemudvikling, programmering og design";
        await InitializeAsync();

        // Act
        var website = Website.Create(
            url,
            "jobnet",
            [searchTerm]).Value;

        await DbContext.Websites.AddAsync(website);
        await DbContext.SaveChangesAsync();

        // Assert
        var savedWebsite = await DbContext.Websites.FirstOrDefaultAsync();
        Assert.IsNotNull(savedWebsite);
        Assert.AreEqual("jobnet", savedWebsite.ShortName);
    }
}