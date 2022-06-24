using Microsoft.EntityFrameworkCore.Migrations;

namespace MTracking.DAL.Migrations
{
    public partial class AddUserPinsToFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserFilePins_FileId",
                table: "UserFilePins",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFilePins_Files_FileId",
                table: "UserFilePins",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFilePins_Files_FileId",
                table: "UserFilePins");

            migrationBuilder.DropIndex(
                name: "IX_UserFilePins_FileId",
                table: "UserFilePins");
        }
    }
}
