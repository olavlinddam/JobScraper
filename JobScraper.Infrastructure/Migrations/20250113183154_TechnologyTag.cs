using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JobScraper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TechnologyTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "JobListings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TechnologyTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnologyTag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobListingTechnologyTag",
                columns: table => new
                {
                    JobListingsId = table.Column<int>(type: "integer", nullable: false),
                    TechnologyTagsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobListingTechnologyTag", x => new { x.JobListingsId, x.TechnologyTagsId });
                    table.ForeignKey(
                        name: "FK_JobListingTechnologyTag_JobListings_JobListingsId",
                        column: x => x.JobListingsId,
                        principalTable: "JobListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobListingTechnologyTag_TechnologyTag_TechnologyTagsId",
                        column: x => x.TechnologyTagsId,
                        principalTable: "TechnologyTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobListingTechnologyTag_TechnologyTagsId",
                table: "JobListingTechnologyTag",
                column: "TechnologyTagsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobListingTechnologyTag");

            migrationBuilder.DropTable(
                name: "TechnologyTag");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "JobListings");
        }
    }
}
