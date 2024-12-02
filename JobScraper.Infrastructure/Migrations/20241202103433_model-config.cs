using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobScraper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modelconfig : Migration
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

            migrationBuilder.DropColumn(
                name: "WebsiteId",
                table: "ScrapingError");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobListing_City_CityId",
                table: "JobListing");

            migrationBuilder.DropForeignKey(
                name: "FK_JobListing_Websites_WebsiteId",
                table: "JobListing");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteId",
                table: "ScrapingError",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_JobListing_City_CityId",
                table: "JobListing",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobListing_Websites_WebsiteId",
                table: "JobListing",
                column: "WebsiteId",
                principalTable: "Websites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
