using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_Data.Migrations
{
    /// <inheritdoc />
    public partial class addisactivemeeting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "meetings",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "meetingforms",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "meetings");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "meetingforms");
        }
    }
}
