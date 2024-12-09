using ErrorOr;
using JobScraper.Application.Features.Scraping.Scrapers;
using JobScraper.Contracts.Requests.Scraping;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ScrapingResult = JobScraper.Contracts.Requests.Scraping.ScrapingResult;

namespace JobScraper.Infrastructure.Scrapers;

public class JobnetScraper : IJobnetScraper
{
    private IWebDriver? _driver;
    private readonly ILogger<JobnetScraper> _logger;

    public JobnetScraper(ILogger<JobnetScraper> logger)
    {
        _logger = logger;
    }

    private IWebDriver? GetDriver()
    {
        if (_driver != null) return _driver;

        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("headless");
        _driver = new ChromeDriver(chromeOptions);

        if (_driver == null)
        {
            throw new InvalidOperationException();
        }

        return _driver;
    }

    public async Task<List<ScrapingResult>> ScrapePageAsync(ScrapeRequest scrapeRequest,
        CancellationToken cancellationToken)
    {
        var scrapingResultsFromAllSearchTerms = new List<ScrapingResult>();
        try
        {
            var urls = BuildUrls(scrapeRequest);
            foreach (var url in urls)
            {
                var scrapingResultForSpecificSearchTerm = await StartScrapeAsync(url, cancellationToken);
                scrapingResultsFromAllSearchTerms.AddRange(scrapingResultForSpecificSearchTerm.Value);
            }

            return scrapingResultsFromAllSearchTerms;
        }
        catch (WebDriverTimeoutException e)
        {
            var scrapingResult = new ScrapingResult(
                ScrapedJob: null,
                FailedJobScrape: new FailedJobScrape(
                    Scraper: "jobnet",
                    TimeStamp: DateTime.Now,
                    Message: e.Message,
                    StackTrace: e.StackTrace ?? "No stack trace found.",
                    Type: "WebdriverTimeoutError"
                )
            );
            scrapingResultsFromAllSearchTerms.Add(scrapingResult);
            return scrapingResultsFromAllSearchTerms;
        }
        catch (NoSuchElementException e)
        {
            _logger.LogError("No such element found at {siteTitle}: {e}", _driver?.Title, e);
            throw;
        }
        catch (OperationCanceledException e)
        {
            _logger.LogError("The operation was canceled due to: {e}.", e);
            throw;
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError("Failed to initialize the web driver for {siteTitle}: {e}", _driver?.Title, e);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occured when scraping {website}: {e}", scrapeRequest.WebsiteBaseUrl, e);
            throw;
        }
    }

    private async Task<ErrorOr<List<ScrapingResult>>> StartScrapeAsync(string url, CancellationToken cancellationToken)
    {
        var scrapingResults = new List<ScrapingResult>();

        var listings = await GetListings(url, cancellationToken);
        if (listings.Count == 0)
        {
            var failedScrape = new ScrapingResult(
                ScrapedJob: null,
                FailedJobScrape: new FailedJobScrape(
                    Scraper: "jobnet",
                    Message: $"No job listings found for {url}",
                    StackTrace: null,
                    TimeStamp: DateTime.Now,
                    Type: "InvalidInput"
                )
            );
            scrapingResults.Add(failedScrape);
            return scrapingResults;
        }

        foreach (var listing in listings)
        {
            var scrapingResult = ParseListing(listing);
            scrapingResults.Add(scrapingResult);
        }

        return scrapingResults;
    }

    internal static List<string> BuildUrls(ScrapeRequest scrapeRequest)
    {
        var urls = new List<string>();
        foreach (var searchTerm in scrapeRequest.SearchTerms)
        {
            var parameter = EncodeSearchTerm(searchTerm);
            var url = $"{scrapeRequest.WebsiteBaseUrl}&{string.Join("&", parameter)}";
            urls.Add(url);
        }

        return urls;
    }

    internal static string EncodeSearchTerm(string searchTerm)
    {
        if (!searchTerm.Contains(' '))
        {
            return searchTerm;
        }

        // Replace spaces with %2520
        var encodedSpaces = searchTerm.Replace(" ", "%2520");

        // Add %2522 for quotes at start and end
        return $"%2522{encodedSpaces}%2522";
    }

    internal static string EncodeLocation(string location)
    {
        if (!location.Contains(' '))
        {
            return location;
        }

        var firstEncode = Uri.EscapeDataString(location);

        return firstEncode.Replace("%", "%25"); //location.Replace(" ", "%2520");
    }

    private async Task<IReadOnlyCollection<IWebElement>> GetListings(string url, CancellationToken cancellationToken)
    {
        var driver = GetDriver();
        await driver.Navigate().GoToUrlAsync(url).WaitAsync(cancellationToken); // null check is in GetDriver()
        HandleCookiePopUp(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        var listings = driver.FindElements(By.ClassName("job-ad-summary"));
        return listings;
    }

    private void HandleCookiePopUp(CancellationToken cancellationToken)
    {
        try
        {
            // Use WebDriverWait to handle waiting for the cookie pop up
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

            // Wait until either the pop up is found or timeout occurs
            var cookiePopup = wait.Until(d =>
            {
                // Check if the operation was cancelled during wait
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    return d.FindElement(By.Id("cc-b-custom"));
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });

            if (cookiePopup == null) return;

            cookiePopup.Click();
            _logger.LogDebug("Clicking on cookie popup");
        }
        catch (WebDriverTimeoutException)
        {
            _logger.LogDebug("No cookie popup found within timeout - continuing");
            // Just continue if no popup found
        }
        catch (OperationCanceledException)
        {
            throw; // Re-throw cancellation
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected error handling cookie popup - continuing");
            // Continue despite other errors with the popup
        }
    }

    internal static ScrapingResult ParseListing(IWebElement listing)
    {
        try
        {
            var anchor = listing.FindElement(By.TagName("a"));
            var href = anchor.GetAttribute("href");
            var title = listing.FindElement(By.TagName("h2")).Text;
            var company = listing.FindElement(By.TagName("h3")).Text;
            var description = listing.FindElement(By.CssSelector("div[class='job-ad-external text-small ng-binding']"))
                .Text;

            var jobAdDetails = listing.FindElement(By.ClassName("job-ad-deadlines-inside"));
            var datePublished = jobAdDetails.FindElement(By.CssSelector("span[data-ng-show='item.PostingCreated']"))
                .Text;
            var workHours = jobAdDetails.FindElement(By.CssSelector("div.job-ad-workhours>span.job-ad-footer-label"))
                .Text;
            var expirationDate = jobAdDetails
                .FindElement(By.CssSelector("div.job-ad-ansogningsfrist>span.job-ad-footer-label")).Text;
            var location = jobAdDetails.FindElement(By.ClassName("job-ad-location")).Text;

            return new ScrapingResult(
                ScrapedJob: new ScrapedJobData(
                    Title: title,
                    CompanyName: company,
                    Description: description,
                    Location: location,
                    DatePublished: datePublished,
                    ExpirationDate: expirationDate,
                    WorkHours: workHours,
                    Url: href,
                    ScrapedDate: DateTime.Today),
                    FailedJobScrape: null);
        }
        catch (Exception e)
        {
            return CreateFailedScrape(e.Message, e.StackTrace, "ParseError");
        }
    }
    
private static ScrapingResult CreateFailedScrape(string message, string? stackTrace, string type) =>
    new(
        ScrapedJob: null,
        FailedJobScrape: new FailedJobScrape(
        Scraper: "jobnet",
        TimeStamp: DateTime.Now,
        Message: message,
        StackTrace: stackTrace,
        Type: type
    ));


    public void Dispose() => _driver?.Dispose();
}