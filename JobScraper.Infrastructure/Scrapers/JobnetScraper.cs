using System.Globalization;
using ErrorOr;
using JobScraper.Application.Features.Scraping.Scrapers;
using JobScraper.Contracts.Requests.Scraping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using ScrapingResult = JobScraper.Contracts.Requests.Scraping.ScrapingResult;

namespace JobScraper.Infrastructure.Scrapers;

public class JobnetScraper : IJobnetScraper
{
    private IWebDriver? _driver;
    private readonly ILogger<JobnetScraper> _logger;
    private readonly IConfiguration _configuration;

    public JobnetScraper(ILogger<JobnetScraper> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    private IWebDriver? GetDriver()
    {
        _logger.LogInformation("Attempting to get driver");
        if (_driver != null) return _driver;

        var seleniumUrl = _configuration["SELENIUM_URL"] ??
                          throw new InvalidOperationException("SeleniumUrl not configured");
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--headless");

        _driver = new RemoteWebDriver(new Uri(seleniumUrl), chromeOptions);

        if (_driver == null)
        {
            throw new InvalidOperationException("Failed to initialize chrome driver");
        }

        return _driver;
    }

    public async Task<List<ScrapingResult>> ScrapePageAsync(ScrapeRequest scrapeRequest,
        CancellationToken cancellationToken)
    {
        var scrapingResultsFromAllSearchTerms = new List<ScrapingResult>();
        try
        {
            _logger.LogInformation("Started jobnet scraper, scrapePageAsync");
            foreach (var searchTerm in scrapeRequest.SearchTerms)
            {
                var url = BuildUrl(searchTerm, scrapeRequest.WebsiteBaseUrl);
                var scrapingResultForSpecificSearchTerm = await StartScrapeAsync(url, cancellationToken);
                scrapingResultForSpecificSearchTerm =
                    AddSearchTermAndBaseUrlToScrapingResult(scrapingResultForSpecificSearchTerm,
                        scrapeRequest.WebsiteBaseUrl, searchTerm);

                scrapingResultsFromAllSearchTerms.AddRange(scrapingResultForSpecificSearchTerm.Value);
            }

            return scrapingResultsFromAllSearchTerms;
        }
        catch (WebDriverTimeoutException e)
        {
            var scrapingResult =
                CreateFailedScrape(e.Message, e.StackTrace ?? "No stack trace found", "DriverTimeOutError");
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
            _logger.LogError("An unexpected error occured when scraping {website}: {e}", scrapeRequest.WebsiteBaseUrl,
                e);
            throw;
        }
    }

    internal static ErrorOr<List<ScrapingResult>> AddSearchTermAndBaseUrlToScrapingResult(
        ErrorOr<List<ScrapingResult>> scrapingResultForSpecificSearchTerm, string baseurl, string searchTerm)
    {
        foreach (var successfulScrape in scrapingResultForSpecificSearchTerm.Value.Select(x => x.ScrapedJobData))
        {
            if (successfulScrape == null) continue;
            successfulScrape.SearchTerm = searchTerm;
            successfulScrape.WebsiteBaseUrl = baseurl;
        }

        return scrapingResultForSpecificSearchTerm;
    }

    private async Task<ErrorOr<List<ScrapingResult>>> StartScrapeAsync(string url, CancellationToken cancellationToken)
    {
        var scrapingResults = new List<ScrapingResult>();

        var listings = await GetListings(url, cancellationToken);
        _logger.LogInformation("Found {x} listings", listings.Count);
        if (listings.Count == 0)
        {
            _logger.LogInformation("No listings found for {url}", url);
            var message = $"no job listings found for {url}";
            var failedScrape = CreateFailedScrape(message, null, "InvalidInput");
            scrapingResults.Add(failedScrape);
            return scrapingResults;
        }
        _logger.LogInformation("Found {x} job listings", listings.Count);

        foreach (var listing in listings)
        {
            var scrapingResult = ParseListing(listing);
            scrapingResults.Add(scrapingResult);
        }

        return scrapingResults;
    }

    internal static string BuildUrl(string searchTerm, string baseUrl)
    {
        var parameter = $"SearchString={EncodeSearchTerm(searchTerm)}";
        var url = $"{baseUrl}&{string.Join("&", parameter)}";
        return url;
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
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

            var cookiePopup = wait.Until(d =>
            {
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
        catch (OperationCanceledException e)
        {
            _logger.LogError("The operation was canceled due to cancellation: {e}", e);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogWarning("Unexpected error handling cookie popup, continuing: {e}", e);
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
            var expirationDate = jobAdDetails.FindElement(By.CssSelector("div.job-ad-ansogningsfrist")).Text;
            var location = jobAdDetails.FindElement(By.ClassName("job-ad-location")).Text;

            return new ScrapingResult()
            {
                ScrapedJobData = new ScrapedJobData()
                {
                    Title = title,
                    CompanyName = company,
                    Description = description,
                    Location = location,
                    DatePublished = ParseDatePublished(datePublished),
                    ExpirationDate = ParseExpirationDate(expirationDate),
                    WorkHours = workHours,
                    Url = href,
                    ScrapedDate = DateTime.Today
                },
                FailedJobScrape = null
            };
        }
        catch (Exception e)
        {
            return CreateFailedScrape(e.Message, e.StackTrace, "ParseError");
        }
    }

    internal static ScrapingResult CreateFailedScrape(string message, string? stackTrace, string type) =>
        new()
        {
            ScrapedJobData = null,
            FailedJobScrape = new FailedJobScrape
            {
                Scraper = "jobnet",
                TimeStamp = DateTime.Now,
                Message = message,
                StackTrace = stackTrace,
                Type = type
            }
        };

internal static DateTime? ParseExpirationDate(string fullText)
{
    var split = fullText.Split(':');

    if (split.Length <= 1) return null;
    
    var datePart = split[1].Trim();
    var daDk = CultureInfo.GetCultureInfo("da-DK");
        
    var possibleFormats = new[] { "dd. MMMM yyyy", "d. MMMM yyyy" };
        
    if (DateTime.TryParseExact(
            datePart, 
            possibleFormats, 
            daDk, 
            DateTimeStyles.None, 
            out var parsedDate))
    {
        return parsedDate;
    }
    return null;
}

    internal static DateTime? ParseDatePublished(string fullText)
    {
        fullText = fullText.Trim();
        if (fullText.EndsWith("-"))
        {
            // Fjern sidste tegn (bindestreg)
            fullText = fullText.Substring(0, fullText.Length - 1).Trim();
        }

        // Forsøg at parse datoen. Den danske formattering forventer fx "dd. MMMM yyyy"
        // men hvis du vil være mere fleksibel, kan du justere til "d. MMMM yyyy" eller bruge TryParseExact.
        try
        {
            var daDk = CultureInfo.GetCultureInfo("da-DK");
            // Eksempel: "20. december 2024" → day=20, month=december, year=2024
            return DateTime.ParseExact(fullText, "dd. MMMM yyyy", daDk);
        }
        catch (FormatException)
        {
            // Hvis formateringen ikke passer, kan vi returnere null eller kaste en exception
            return null;
        }
    }

    public void Dispose() => _driver?.Dispose();
}