using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JobScraper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class JobListingUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobListing_City_CityId",
                table: "JobListing");

            migrationBuilder.DropForeignKey(
                name: "FK_JobListing_Websites_WebsiteId",
                table: "JobListing");

            migrationBuilder.DropForeignKey(
                name: "FK_ScrapingErrorWebsite_ScrapingError_ScrapingErrorsId",
                table: "ScrapingErrorWebsite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScrapingError",
                table: "ScrapingError");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobListing",
                table: "JobListing");

            migrationBuilder.DropPrimaryKey(
                name: "PK_City",
                table: "City");

            migrationBuilder.RenameTable(
                name: "ScrapingError",
                newName: "ScrapingErrors");

            migrationBuilder.RenameTable(
                name: "JobListing",
                newName: "JobListings");

            migrationBuilder.RenameTable(
                name: "City",
                newName: "Cities");

            migrationBuilder.RenameColumn(
                name: "Uri",
                table: "JobListings",
                newName: "Url");

            migrationBuilder.RenameIndex(
                name: "IX_JobListing_WebsiteId",
                table: "JobListings",
                newName: "IX_JobListings_WebsiteId");

            migrationBuilder.RenameIndex(
                name: "IX_JobListing_CityId",
                table: "JobListings",
                newName: "IX_JobListings_CityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScrapingErrors",
                table: "ScrapingErrors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobListings",
                table: "JobListings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cities",
                table: "Cities",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SearchTerms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastUsed = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MatchingJobsCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchTerms", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_JobListingSearchTerm_SearchTermsId",
                table: "JobListingSearchTerm",
                column: "SearchTermsId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchTerms_Value",
                table: "SearchTerms",
                column: "Value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SearchTermWebsite_WebsitesId",
                table: "SearchTermWebsite",
                column: "WebsitesId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobListings_Cities_CityId",
                table: "JobListings",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobListings_Websites_WebsiteId",
                table: "JobListings",
                column: "WebsiteId",
                principalTable: "Websites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScrapingErrorWebsite_ScrapingErrors_ScrapingErrorsId",
                table: "ScrapingErrorWebsite",
                column: "ScrapingErrorsId",
                principalTable: "ScrapingErrors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobListings_Cities_CityId",
                table: "JobListings");

            migrationBuilder.DropForeignKey(
                name: "FK_JobListings_Websites_WebsiteId",
                table: "JobListings");

            migrationBuilder.DropForeignKey(
                name: "FK_ScrapingErrorWebsite_ScrapingErrors_ScrapingErrorsId",
                table: "ScrapingErrorWebsite");

            migrationBuilder.DropTable(
                name: "JobListingSearchTerm");

            migrationBuilder.DropTable(
                name: "SearchTermWebsite");

            migrationBuilder.DropTable(
                name: "SearchTerms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScrapingErrors",
                table: "ScrapingErrors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobListings",
                table: "JobListings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cities",
                table: "Cities");

            migrationBuilder.RenameTable(
                name: "ScrapingErrors",
                newName: "ScrapingError");

            migrationBuilder.RenameTable(
                name: "JobListings",
                newName: "JobListing");

            migrationBuilder.RenameTable(
                name: "Cities",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "JobListing",
                newName: "Uri");

            migrationBuilder.RenameIndex(
                name: "IX_JobListings_WebsiteId",
                table: "JobListing",
                newName: "IX_JobListing_WebsiteId");

            migrationBuilder.RenameIndex(
                name: "IX_JobListings_CityId",
                table: "JobListing",
                newName: "IX_JobListing_CityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScrapingError",
                table: "ScrapingError",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobListing",
                table: "JobListing",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_City",
                table: "City",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobListing_City_CityId",
                table: "JobListing",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobListing_Websites_WebsiteId",
                table: "JobListing",
                column: "WebsiteId",
                principalTable: "Websites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScrapingErrorWebsite_ScrapingError_ScrapingErrorsId",
                table: "ScrapingErrorWebsite",
                column: "ScrapingErrorsId",
                principalTable: "ScrapingError",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
