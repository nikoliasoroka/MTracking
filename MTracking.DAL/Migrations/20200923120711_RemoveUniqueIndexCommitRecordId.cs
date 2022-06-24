using Microsoft.EntityFrameworkCore.Migrations;

namespace MTracking.DAL.Migrations
{
    public partial class RemoveUniqueIndexCommitRecordId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeLogs_CommitRecordId",
                table: "TimeLogs");

            migrationBuilder.CreateIndex(
                name: "IX_TimeLogs_CommitRecordId",
                table: "TimeLogs",
                column: "CommitRecordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeLogs_CommitRecordId",
                table: "TimeLogs");

            migrationBuilder.CreateIndex(
                name: "IX_TimeLogs_CommitRecordId",
                table: "TimeLogs",
                column: "CommitRecordId",
                unique: true);
        }
    }
}
