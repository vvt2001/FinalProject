using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_Data.Migrations
{
    /// <inheritdoc />
    public partial class addvotinghistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "votinghistories",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    attendee_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    meetingform_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    meetingtime_id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_votinghistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_votinghistories_attendees_attendee_id",
                        column: x => x.attendee_id,
                        principalTable: "attendees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_votinghistories_meetingforms_meetingform_id",
                        column: x => x.meetingform_id,
                        principalTable: "meetingforms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_votinghistories_meetingtimes_meetingtime_id",
                        column: x => x.meetingtime_id,
                        principalTable: "meetingtimes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_votinghistories_attendee_id",
                table: "votinghistories",
                column: "attendee_id");

            migrationBuilder.CreateIndex(
                name: "IX_votinghistories_meetingform_id",
                table: "votinghistories",
                column: "meetingform_id");

            migrationBuilder.CreateIndex(
                name: "IX_votinghistories_meetingtime_id",
                table: "votinghistories",
                column: "meetingtime_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "votinghistories");
        }
    }
}
