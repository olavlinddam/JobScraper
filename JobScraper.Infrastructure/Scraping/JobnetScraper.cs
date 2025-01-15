using System.Globalization;
using JobScraper.Application.Features.Scraping.Dtos;
using JobScraper.Application.Features.Scraping.Scrapers;
using JobScraper.Infrastructure.Scrapers;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace JobScraper.Infrastructure.Scraping;

public class JobnetScraper : IJobnetScraper
{
        private readonly WebDriverFactory _driverFactory;
        private readonly ILogger<JobnetScraper> _logger;

        public JobnetScraper(WebDriverFactory driverFactory, ILogger<JobnetScraper> logger)
        {
                _driverFactory = driverFactory;
                _logger = logger;
        }

        public async Task<List<ScrapingResult>> ScrapeAsync(ScrapeRequest scrapeRequest,
                CancellationToken cancellationToken)
        {
                _logger.LogInformation("Started jobnet scraper to scrape search term: [{searchTerm}]", scrapeRequest.SearchTerm);
                var results = new List<ScrapingResult>();
                try
                {
                        var searchResults = await ProcessSearchTermAsync(scrapeRequest, cancellationToken);
                        searchResults = AddSearchTermAndBaseUrlToScrapingResult(searchResults,
                                scrapeRequest.WebsiteBaseUrl, scrapeRequest.SearchTerm);
                        results.AddRange(searchResults);
                }
                catch (OperationCanceledException e)
                {
                        _logger.LogError("The operation was canceled due to cancellation: {e}", e);
                        throw;
                }
                catch (Exception e)
                {
                        _logger.LogError(e, "Failed to scrape search term: [{SearchTerm}], due to: [{error}]",
                                scrapeRequest.SearchTerm,
                                e.Message);

                        // dont throw since we want to continue with the rest of the search terms.
                }

                return results;
        }

        private async Task<List<ScrapingResult>> ProcessSearchTermAsync(ScrapeRequest request,
                CancellationToken cancellationToken)
        {
                var scrapingResults = new List<ScrapingResult>();
                var offset = 0;
                var hasNextPage = true;

                while (hasNextPage && !cancellationToken.IsCancellationRequested)
                {
                        PageProcessingResult pageResult;
                        var url = BuildUrl(request.SearchTerm, request.WebsiteBaseUrl, offset);
                        using (var driver = _driverFactory.CreateDriver())
                        {
                                pageResult = await ProcessListingsPageAsync(driver, url, cancellationToken);
                                if (!pageResult.IsSuccess)
                                        break;

                                scrapingResults.AddRange(pageResult.ScrapingResults);
                        }

                        // Jobnet shows 20 listings per page
                        hasNextPage = pageResult.HasNextPage &&
                                      !HasFoundExistingListing(pageResult.ScrapingResults, request.LatestScrapedUrl);
                        offset += 20;
                }

                return scrapingResults;
        }

        private bool HasFoundExistingListing(List<ScrapingResult> pageResultScrapingResults, string? lastestScrapedUrl)
        {
                return lastestScrapedUrl != null &&
                       pageResultScrapingResults.Select(r => r.SuccessFullScrape.Url).Contains(lastestScrapedUrl);
        }

        private async Task<PageProcessingResult> ProcessListingsPageAsync(IWebDriver driver, string url,
                CancellationToken cancellationToken)
        {
                await driver.Navigate().GoToUrlAsync(url).WaitAsync(cancellationToken);
                await HandleCookiePopUp(driver, cancellationToken);
                var listings = driver.FindElements(By.ClassName("job-ad-summary"));

                if (listings.Count == 0)
                        return new PageProcessingResult(false, false, []);

                var hasNextPage = HasNextPageButton(driver);

                var listingProcessingResults = ProcessListings(listings);
                if (listingProcessingResults.Count == 0)
                        return new PageProcessingResult(false, false, []);

                var finalScrapingResults = await ProcessHrefs(listingProcessingResults, driver, cancellationToken);

                return new PageProcessingResult(
                        IsSuccess: true,
                        HasNextPage: hasNextPage && listings.Count == 20,
                        ScrapingResults: finalScrapingResults);
        }

        private List<ListingProcessingResult> ProcessListings(IReadOnlyCollection<IWebElement> pageListings)
        {
                var listingProcessingResults = new List<ListingProcessingResult>();
                try
                {
                        listingProcessingResults.AddRange(pageListings.Select(ExtractListingData));
                        return listingProcessingResults;
                }
                catch (Exception e)
                {
                        return [];
                }
        }

        private async Task<List<ScrapingResult>> ProcessHrefs(List<ListingProcessingResult> listingProcessingResults,
                IWebDriver driver,
                CancellationToken cancellationToken)
        {
                var scrapingResults = new List<ScrapingResult>();
                foreach (var listingProcessingResult in listingProcessingResults)
                {
                        try
                        {
                                await driver.Navigate().GoToUrlAsync(listingProcessingResult.Href).WaitAsync(cancellationToken);
                                var html = driver.PageSource;
                                scrapingResults.Add(CreateSuccessFullScrape(listingProcessingResult, html));
                        }
                        catch (Exception e)
                        {
                                _logger.LogError(e, "Failed to scrape listing at {Href}", listingProcessingResult.Href);
                                scrapingResults.Add(CreateFailedScrape(e.Message, e.StackTrace));
                        }
                }

                return scrapingResults;
        }

        private ListingProcessingResult ExtractListingData(IWebElement pageListing)
        {
                var href = pageListing.FindElement(By.TagName("a")).GetAttribute("href");
                var title = pageListing.FindElement(By.TagName("h2")).Text;
                var company = pageListing.FindElement(By.TagName("h3")).Text;
                var jobAdDetails = pageListing.FindElement(By.ClassName("job-ad-deadlines-inside"));
                var expirationDate = jobAdDetails.FindElement(By.CssSelector("div.job-ad-ansogningsfrist")).Text;
                var datePublished = jobAdDetails.FindElement(By.CssSelector("span[data-ng-show='item.PostingCreated']"))
                        .Text;
                var location = jobAdDetails.FindElement(By.ClassName("job-ad-location")).Text;
                return new ListingProcessingResult(href, title, company, datePublished, expirationDate, location);
        }

        private async Task HandleCookiePopUp(IWebDriver driver, CancellationToken cancellationToken)
        {
                try
                {
                        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                        var cookiePopup = await Task.Run(() => wait.Until(d =>
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
                        }), cancellationToken);

                        cookiePopup?.Click();
                }
                catch (WebDriverTimeoutException)
                {
                        _logger.LogDebug("No cookie popup found within timeout");
                }
        }

        private bool HasNextPageButton(IWebDriver driver)
        {
                try
                {
                        var paginationElement = driver.FindElement(By.CssSelector("nav.paging.ng-isolate-scope"));
                        return paginationElement?.FindElements(By.CssSelector("li a")).FirstOrDefault() != null;
                }
                catch (NoSuchElementException)
                {
                        return false;
                }
        }

        private static string BuildUrl(string searchTerm, string baseUrl, int offset)
        {
                var encodedTerm = searchTerm.Contains(' ')
                        ? $"%2522{searchTerm.Replace(" ", "%2520")}%2522"
                        : searchTerm;

                return $"{baseUrl}SearchString={encodedTerm}&Offset={offset}&SortValue=BestMatch";
        }

        internal static List<ScrapingResult> AddSearchTermAndBaseUrlToScrapingResult(
                List<ScrapingResult> scrapingResult, string baseurl, string searchTerm)
        {
                foreach (var successfulScrape in scrapingResult.Select(x => x.SuccessFullScrape))
                {
                        if (successfulScrape == null) continue;
                        successfulScrape.SearchTerm = searchTerm;
                        successfulScrape.WebsiteBaseUrl = baseurl;
                }

                return scrapingResult;
        }

        internal static ScrapingResult CreateFailedScrape(string message, string? stackTrace) => new()
        {
                SuccessFullScrape = null,
                FailedJobScrape = new FailedJobScrape
                {
                        Scraper = "jobnet",
                        TimeStamp = DateTime.UtcNow,
                        Message = message,
                        StackTrace = stackTrace,
                }
        };

        internal static ScrapingResult CreateSuccessFullScrape(ListingProcessingResult listingProcessingResult,
                string articleHtml) => new()
        {
                SuccessFullScrape = new SuccessFullScrape
                {
                        Title = listingProcessingResult.Title,
                        Url = listingProcessingResult.Href,
                        DatePublished = ParseDatePublished(listingProcessingResult.DatePublished),
                        CompanyName = listingProcessingResult.CompanyName,
                        Location = listingProcessingResult.Location,
                        ExpirationDate = ParseExpirationDate(listingProcessingResult.ExpirationDate),
                        ArticleHtml = articleHtml,
                        ScrapedDate = DateTime.UtcNow,
                },
                FailedJobScrape = null
        };

        internal static DateTime? ParseDatePublished(string fullText)
        {
                fullText = fullText.Trim();
                if (fullText.EndsWith("-"))
                {
                        fullText = fullText.Substring(0, fullText.Length - 1).Trim();
                }

                try
                {
                        var daDk = CultureInfo.GetCultureInfo("da-DK");
                        // Eksempel: "20. december 2024" → day=20, month=december, year=2024
                        return DateTime.ParseExact(fullText, "dd. MMMM yyyy", daDk);
                }
                catch (FormatException)
                {
                        return null;
                }
        }

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

        public void Dispose()
        {
        }
}