using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace MTracking.DAL.Migrations
{
    public partial class ResetSynchToTopic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSynchronize",
                table: "TimeLogs",
                nullable: true,
                defaultValue: true);

            migrationBuilder.Sql("UPDATE TimeLogs SET IsSynchronize = 1");
            
            migrationBuilder.DropColumn(
                name: "IsSynchronize",
                table: "Topics");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSynchronize",
                table: "TimeLogs");
        }
    }
}
