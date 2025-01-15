using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace JobScraper.Infrastructure.Scrapers;

public class WebDriverFactory
{
    private readonly ILogger<WebDriverFactory> _logger;
    private readonly IConfiguration _configuration;

    public WebDriverFactory(IConfiguration configuration, ILogger<WebDriverFactory> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public IWebDriver CreateDriver()
    {
        try
        {
            _logger.LogInformation("Attempting to create driver");
            var seleniumUrl = _configuration["SELENIUM_URL"] ??
                              throw new InvalidOperationException("SeleniumUrl not configured");
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");

            var driver = new RemoteWebDriver(new Uri(seleniumUrl), chromeOptions);

            if (driver == null)
            {
                throw new InvalidOperationException("Failed to initialize chrome driver");
            }

            return driver;
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError("Failed to initialize driver due to: [{message}]", e.Message);
            throw;
        }
    }
}