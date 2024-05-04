using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_Data.Migrations
{
    /// <inheritdoc />
    public partial class fixattendee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendee_meetingforms");

            migrationBuilder.DropTable(
                name: "attendee_meetings");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "attendees",
                newName: "name");

            migrationBuilder.AddColumn<string>(
                name: "meeting_id",
                table: "attendees",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "meetingform_id",
                table: "attendees",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_attendees_meeting_id",
                table: "attendees",
                column: "meeting_id");

            migrationBuilder.CreateIndex(
                name: "IX_attendees_meetingform_id",
                table: "attendees",
                column: "meetingform_id");

            migrationBuilder.AddForeignKey(
                name: "FK_attendees_meetingforms_meetingform_id",
                table: "attendees",
                column: "meetingform_id",
                principalTable: "meetingforms",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_attendees_meetings_meeting_id",
                table: "attendees",
                column: "meeting_id",
                principalTable: "meetings",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attendees_meetingforms_meetingform_id",
                table: "attendees");

            migrationBuilder.DropForeignKey(
                name: "FK_attendees_meetings_meeting_id",
                table: "attendees");

            migrationBuilder.DropIndex(
                name: "IX_attendees_meeting_id",
                table: "attendees");

            migrationBuilder.DropIndex(
                name: "IX_attendees_meetingform_id",
                table: "attendees");

            migrationBuilder.DropColumn(
                name: "meeting_id",
                table: "attendees");

            migrationBuilder.DropColumn(
                name: "meetingform_id",
                table: "attendees");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "attendees",
                newName: "username");

            migrationBuilder.CreateTable(
                name: "attendee_meetingforms",
                columns: table => new
                {
                    attendee_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    meetingform_id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendee_meetingforms", x => new { x.attendee_id, x.meetingform_id });
                    table.ForeignKey(
                        name: "FK_attendee_meetingforms_attendees_attendee_id",
                        column: x => x.attendee_id,
                        principalTable: "attendees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attendee_meetingforms_meetingforms_meetingform_id",
                        column: x => x.meetingform_id,
                        principalTable: "meetingforms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attendee_meetings",
                columns: table => new
                {
                    attendee_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    meeting_id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendee_meetings", x => new { x.attendee_id, x.meeting_id });
                    table.ForeignKey(
                        name: "FK_attendee_meetings_attendees_attendee_id",
                        column: x => x.attendee_id,
                        principalTable: "attendees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attendee_meetings_meetings_meeting_id",
                        column: x => x.meeting_id,
                        principalTable: "meetings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attendee_meetingforms_meetingform_id",
                table: "attendee_meetingforms",
                column: "meetingform_id");

            migrationBuilder.CreateIndex(
                name: "IX_attendee_meetings_meeting_id",
                table: "attendee_meetings",
                column: "meeting_id");
        }
    }
}
