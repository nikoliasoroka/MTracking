using Microsoft.EntityFrameworkCore.Migrations;

namespace MTracking.DAL.Migrations
{
    public partial class AddColumnsToTopics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillingCodeId",
                table: "Topics",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Detail",
                table: "Topics",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsNotary",
                table: "Topics",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitType",
                table: "Topics",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingCodeId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Detail",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "IsNotary",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "UnitType",
                table: "Topics");
        }
    }
}
