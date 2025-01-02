# Job Posting Scraper

A .NET-based web scraping application that aggregates danish job postings from multiple sources into a centralized database. Makes job searching more efficient by providing a single place to search and filter job listings from various websites.

## Features

- Automated job posting collection from multiple sources
- Structured storage of job listings in PostgreSQL
- Rate limiting and scraping policies per website
- Geographic data support for job locations
- Error tracking and logging with Serilog
- Clean Architecture implementation
- Support for multiple search parameters (location, job type, distance)

## Tech Stack

- .NET 8
- C#
- PostgreSQL with Entity Framework Core
- Selenium WebDriver for scraping
- Serilog for structured logging
- ErrorOr for functional error handling

## Project Structure

```
JobScraper/
├── src/
│   ├── JobScraper.Api/           # Controllers and API endpoints
│   ├── JobScraper.Application/   # Business logic and interfaces
│   ├── JobScraper.Contracts/     # DTOs and shared models
│   ├── JobScraper.Domain/        # Domain entities and enums
│   └── JobScraper.Infrastructure/# External concerns (DB, HTTP, etc.)
└── tests/                        # Test projects
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL
- Docker and Docker Compose (Docker desktop should be enough)

### Setup

Clone the repository:

```git clone https://github.com/your-repo/job-scraper.git```

```cd job-scraper```

Start the necessary services:

```docker compose up```

Run the application:

```dotnet run --project JobScraper.Api```

Navigate to: 
```http://localhost:5000/swagger/index.html```

Send the following json package to:
```http://localhost:5000/api/Website/create```

```json
{
  "url": "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch",
  "shortName": "jobnet",
  "searchTerms": [
    "Systemudvikling, programmering og design"
  ]
}
```
The application is currently only set up to work with this specific website and shortname. Search terms can be changed but I have not yet tested with other than the provided.

To initiate a scrape go to:
```http://localhost:5000/api/Scraping/StartAllScrapers```

After the scrapea is finished go to:
```http://localhost:5000/api/JobListing/job-listings```

To fetch all listings.

## API Endpoints

Not yet implemented


### Phase 1 (MVP)
- [ ] Complete JobnetScraper implementation (parsing and storing results)
- [ ] Implement deduplication logic
- [ ] Add job search API endpoints
- [ ] Add user interface for displaying job listings
- [ ] Set up compose environment


### Phase 2
- [ ] Set up CI/CD pipeline
- [ ] Add more job sites (create new scrapers)
- [ ] Create admin interface for managing scrapers and viewing errors
- [ ] Add search term tracking and analytics
- [ ] Create documentation for adding new scrapers
- [ ] Add rate limiting middleware
- [ ] Add global error handling middleware
- [ ] Add health checks

