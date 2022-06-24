using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MTracking.DAL.Migrations
{
    public partial class AddImports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Descriptions_Users_UserId",
                table: "Descriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeLogs_Users_UserId",
                table: "TimeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Users_UserId",
                table: "Topics");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TimeLogs",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CommitRecordId",
                table: "Descriptions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Imports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    FileType = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    InsertedRecords = table.Column<int>(nullable: true),
                    UpdatedRecords = table.Column<int>(nullable: true),
                    Performance = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeLogs_CommitRecordId",
                table: "TimeLogs",
                column: "CommitRecordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Descriptions_CommitRecordId",
                table: "Descriptions",
                column: "CommitRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Descriptions_Users_UserId",
                table: "Descriptions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeLogs_Users_UserId",
                table: "TimeLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Users_UserId",
                table: "Topics",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Descriptions_Users_UserId",
                table: "Descriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeLogs_Users_UserId",
                table: "TimeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Users_UserId",
                table: "Topics");

            migrationBuilder.DropTable(
                name: "Imports");

            migrationBuilder.DropIndex(
                name: "IX_TimeLogs_CommitRecordId",
                table: "TimeLogs");

            migrationBuilder.DropIndex(
                name: "IX_Descriptions_CommitRecordId",
                table: "Descriptions");

            migrationBuilder.DropColumn(
                name: "CommitRecordId",
                table: "Descriptions");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TimeLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Descriptions_Users_UserId",
                table: "Descriptions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeLogs_Users_UserId",
                table: "TimeLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Users_UserId",
                table: "Topics",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
