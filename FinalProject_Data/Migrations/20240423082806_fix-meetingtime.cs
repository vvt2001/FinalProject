using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_Data.Migrations
{
    /// <inheritdoc />
    public partial class fixmeetingtime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meetingtimes_meetings_meetingID",
                table: "meetingtimes");

            migrationBuilder.RenameColumn(
                name: "meetingtime_id",
                table: "meetingtimes",
                newName: "meeting_id");

            migrationBuilder.AlterColumn<string>(
                name: "meetingID",
                table: "meetingtimes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_meetingtimes_meetings_meetingID",
                table: "meetingtimes",
                column: "meetingID",
                principalTable: "meetings",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meetingtimes_meetings_meetingID",
                table: "meetingtimes");

            migrationBuilder.RenameColumn(
                name: "meeting_id",
                table: "meetingtimes",
                newName: "meetingtime_id");

            migrationBuilder.AlterColumn<string>(
                name: "meetingID",
                table: "meetingtimes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_meetingtimes_meetings_meetingID",
                table: "meetingtimes",
                column: "meetingID",
                principalTable: "meetings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
