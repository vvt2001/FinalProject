using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_Data.Migrations
{
    /// <inheritdoc />
    public partial class addmeetingattendeetables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attendees",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    meeting_id = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendees", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "meetingforms",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    starttime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    meeting_title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    meeting_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    platform = table.Column<int>(type: "int", nullable: false),
                    trangthai = table.Column<int>(type: "int", nullable: false),
                    meeting_id = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meetingforms", x => x.ID);
                });

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
                name: "meetings",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    starttime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    meeting_title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    meeting_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    platform = table.Column<int>(type: "int", nullable: false),
                    trangthai = table.Column<int>(type: "int", nullable: false),
                    meetingform_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    meetingtime_id = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meetings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_meetings_meetingforms_meetingform_id",
                        column: x => x.meetingform_id,
                        principalTable: "meetingforms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "meetingtimes",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    meetingID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    meetingform_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    meetingtime_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    trangthai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meetingtimes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_meetingtimes_meetingforms_meetingform_id",
                        column: x => x.meetingform_id,
                        principalTable: "meetingforms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_meetingtimes_meetings_meetingID",
                        column: x => x.meetingID,
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

            migrationBuilder.CreateIndex(
                name: "IX_meetings_meetingform_id",
                table: "meetings",
                column: "meetingform_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_meetingtimes_meetingform_id",
                table: "meetingtimes",
                column: "meetingform_id");

            migrationBuilder.CreateIndex(
                name: "IX_meetingtimes_meetingID",
                table: "meetingtimes",
                column: "meetingID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendee_meetingforms");

            migrationBuilder.DropTable(
                name: "attendee_meetings");

            migrationBuilder.DropTable(
                name: "meetingtimes");

            migrationBuilder.DropTable(
                name: "attendees");

            migrationBuilder.DropTable(
                name: "meetings");

            migrationBuilder.DropTable(
                name: "meetingforms");
        }
    }
}
