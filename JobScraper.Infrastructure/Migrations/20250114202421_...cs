using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobScraper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobListingTechnologyTag_TechnologyTag_TechnologyTagsId",
                table: "JobListingTechnologyTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TechnologyTag",
                table: "TechnologyTag");

            migrationBuilder.RenameTable(
                name: "TechnologyTag",
                newName: "TechnologyTags");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TechnologyTags",
                table: "TechnologyTags",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobListingTechnologyTag_TechnologyTags_TechnologyTagsId",
                table: "JobListingTechnologyTag",
                column: "TechnologyTagsId",
                principalTable: "TechnologyTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobListingTechnologyTag_TechnologyTags_TechnologyTagsId",
                table: "JobListingTechnologyTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TechnologyTags",
                table: "TechnologyTags");

            migrationBuilder.RenameTable(
                name: "TechnologyTags",
                newName: "TechnologyTag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TechnologyTag",
                table: "TechnologyTag",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobListingTechnologyTag_TechnologyTag_TechnologyTagsId",
                table: "JobListingTechnologyTag",
                column: "TechnologyTagsId",
                principalTable: "TechnologyTag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
