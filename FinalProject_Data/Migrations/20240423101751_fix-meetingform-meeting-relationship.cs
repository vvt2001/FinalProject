using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_Data.Migrations
{
    /// <inheritdoc />
    public partial class fixmeetingformmeetingrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meetings_meetingforms_meetingform_id",
                table: "meetings");

            migrationBuilder.DropIndex(
                name: "IX_meetings_meetingform_id",
                table: "meetings");

            migrationBuilder.DropColumn(
                name: "meetingform_id",
                table: "meetings");

            migrationBuilder.DropColumn(
                name: "meeting_id",
                table: "meetingforms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "meetingform_id",
                table: "meetings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "meeting_id",
                table: "meetingforms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_meetings_meetingform_id",
                table: "meetings",
                column: "meetingform_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_meetings_meetingforms_meetingform_id",
                table: "meetings",
                column: "meetingform_id",
                principalTable: "meetingforms",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
