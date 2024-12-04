# Job Posting Scraper

A .NET-based web scraping application that aggregates danish job postings from multiple sources into a centralized database. Makes job searching more efficient by providing a single place to search and filter job listings from various websites.

## Features

- Automated job posting collection from multiple sources
- Structured storage of job listings in PostgreSQL
- Rate limiting and scraping policies per website
- Geographic data support for job locations
- Error tracking and logging with Serilog
- Clean Architecture implementation with three layers
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
- Chrome/Chromium (for Selenium WebDriver)
- User Secrets configured with connection string

### Setup

1. Clone the repository
2. Set up the database connection string in user secrets:
   ```bash
   dotnet user-secrets set "ConnectionStrings:LocalDb" "your-connection-string"
   ```
3. Run database migrations
4. Start the application

## API Endpoints

### Scraping

- POST `/api/scraping/StartAllScrapers` - Start scraping jobs from all configured websites (not fully implemented)

## TODO

- [ ] Complete JobnetScraper implementation (parsing and storing results)
- [ ] Add job search API endpoints
- [ ] Implement deduplication logic
- [ ] Add more job sites (create new scrapers)
- [ ] Set up CI/CD pipeline
- [ ] Add comprehensive test coverage
- [ ] Create admin interface for managing scrapers and viewing errors
- [ ] Add rate limiting middleware
- [ ] Implement robots.txt parsing and respect
- [ ] Add search term tracking and analytics
- [ ] Create documentation for adding new scrapers
