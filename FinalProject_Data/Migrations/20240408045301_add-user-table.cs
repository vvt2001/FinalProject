using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_Data.Migrations
{
    /// <inheritdoc />
    public partial class addusertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "meetingtime_id",
                table: "meetings");

            migrationBuilder.DropColumn(
                name: "meeting_id",
                table: "attendees");

            migrationBuilder.AddColumn<int>(
                name: "vote_count",
                table: "meetingtimes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "owner_id",
                table: "meetings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "starttime",
                table: "meetingforms",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "duration",
                table: "meetingforms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "owner_id",
                table: "meetingforms",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    username = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    trangthai = table.Column<int>(type: "int", nullable: false),
                    hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    salt = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_meetings_owner_id",
                table: "meetings",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_meetingforms_owner_id",
                table: "meetingforms",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_meetingforms_users_owner_id",
                table: "meetingforms",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_meetings_users_owner_id",
                table: "meetings",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meetingforms_users_owner_id",
                table: "meetingforms");

            migrationBuilder.DropForeignKey(
                name: "FK_meetings_users_owner_id",
                table: "meetings");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropIndex(
                name: "IX_meetings_owner_id",
                table: "meetings");

            migrationBuilder.DropIndex(
                name: "IX_meetingforms_owner_id",
                table: "meetingforms");

            migrationBuilder.DropColumn(
                name: "vote_count",
                table: "meetingtimes");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "meetings");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "meetingforms");

            migrationBuilder.AddColumn<string>(
                name: "meetingtime_id",
                table: "meetings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "starttime",
                table: "meetingforms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "duration",
                table: "meetingforms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "meeting_id",
                table: "attendees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
