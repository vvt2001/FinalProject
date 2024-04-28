using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_Data.Migrations
{
    /// <inheritdoc />
    public partial class fixmeetingtime_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meetingtimes_meetings_meetingID",
                table: "meetingtimes");

            migrationBuilder.DropIndex(
                name: "IX_meetingtimes_meetingID",
                table: "meetingtimes");

            migrationBuilder.DropColumn(
                name: "meetingID",
                table: "meetingtimes");

            migrationBuilder.DropColumn(
                name: "meeting_id",
                table: "meetingtimes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "meetingID",
                table: "meetingtimes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "meeting_id",
                table: "meetingtimes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_meetingtimes_meetingID",
                table: "meetingtimes",
                column: "meetingID");

            migrationBuilder.AddForeignKey(
                name: "FK_meetingtimes_meetings_meetingID",
                table: "meetingtimes",
                column: "meetingID",
                principalTable: "meetings",
                principalColumn: "ID");
        }
    }
}
