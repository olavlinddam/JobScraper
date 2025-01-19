using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JobScraper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProtectedWebsite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    Zip = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScrapingErrors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    StackTrace = table.Column<string>(type: "text", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    ErrorType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapingErrors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchTerms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastUsed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MatchingJobsCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchTerms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Websites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastScraped = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Websites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobListings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    PostedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ScrapedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    JobType = table.Column<int>(type: "integer", nullable: false),
                    WebsiteId = table.Column<int>(type: "integer", nullable: false),
                    CityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobListings_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobListings_Websites_WebsiteId",
                        column: x => x.WebsiteId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScrapingErrorWebsite",
                columns: table => new
                {
                    ScrapingErrorsId = table.Column<int>(type: "integer", nullable: false),
                    WebsitesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapingErrorWebsite", x => new { x.ScrapingErrorsId, x.WebsitesId });
                    table.ForeignKey(
                        name: "FK_ScrapingErrorWebsite_ScrapingErrors_ScrapingErrorsId",
                        column: x => x.ScrapingErrorsId,
                        principalTable: "ScrapingErrors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScrapingErrorWebsite_Websites_WebsitesId",
                        column: x => x.WebsitesId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SearchTermWebsite",
                columns: table => new
                {
                    SearchTermsId = table.Column<int>(type: "integer", nullable: false),
                    WebsitesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchTermWebsite", x => new { x.SearchTermsId, x.WebsitesId });
                    table.ForeignKey(
                        name: "FK_SearchTermWebsite_SearchTerms_SearchTermsId",
                        column: x => x.SearchTermsId,
                        principalTable: "SearchTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SearchTermWebsite_Websites_WebsitesId",
                        column: x => x.WebsitesId,
                        principalTable: "Websites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobListingSearchTerm",
                columns: table => new
                {
                    JobListingsId = table.Column<int>(type: "integer", nullable: false),
                    SearchTermsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobListingSearchTerm", x => new { x.JobListingsId, x.SearchTermsId });
                    table.ForeignKey(
                        name: "FK_JobListingSearchTerm_JobListings_JobListingsId",
                        column: x => x.JobListingsId,
                        principalTable: "JobListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobListingSearchTerm_SearchTerms_SearchTermsId",
                        column: x => x.SearchTermsId,
                        principalTable: "SearchTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_CityId",
                table: "JobListings",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_WebsiteId",
                table: "JobListings",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_JobListingSearchTerm_SearchTermsId",
                table: "JobListingSearchTerm",
                column: "SearchTermsId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrapingErrorWebsite_WebsitesId",
                table: "ScrapingErrorWebsite",
                column: "WebsitesId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchTerms_Value",
                table: "SearchTerms",
                column: "Value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SearchTermWebsite_WebsitesId",
                table: "SearchTermWebsite",
                column: "WebsitesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobListingSearchTerm");

            migrationBuilder.DropTable(
                name: "ScrapingErrorWebsite");

            migrationBuilder.DropTable(
                name: "SearchTermWebsite");

            migrationBuilder.DropTable(
                name: "JobListings");

            migrationBuilder.DropTable(
                name: "ScrapingErrors");

            migrationBuilder.DropTable(
                name: "SearchTerms");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Websites");
        }
    }
}
