using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobScraper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ScrapingPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Websites_ScrapingPolicy_ScrapingPolicyId",
                table: "Websites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScrapingPolicy",
                table: "ScrapingPolicy");

            migrationBuilder.DropColumn(
                name: "AllowedPaths",
                table: "ScrapingPolicy");

            migrationBuilder.RenameTable(
                name: "ScrapingPolicy",
                newName: "ScrapingPolicies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScrapingPolicies",
                table: "ScrapingPolicies",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Websites_ScrapingPolicies_ScrapingPolicyId",
                table: "Websites",
                column: "ScrapingPolicyId",
                principalTable: "ScrapingPolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Websites_ScrapingPolicies_ScrapingPolicyId",
                table: "Websites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScrapingPolicies",
                table: "ScrapingPolicies");

            migrationBuilder.RenameTable(
                name: "ScrapingPolicies",
                newName: "ScrapingPolicy");

            migrationBuilder.AddColumn<List<string>>(
                name: "AllowedPaths",
                table: "ScrapingPolicy",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScrapingPolicy",
                table: "ScrapingPolicy",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Websites_ScrapingPolicy_ScrapingPolicyId",
                table: "Websites",
                column: "ScrapingPolicyId",
                principalTable: "ScrapingPolicy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
