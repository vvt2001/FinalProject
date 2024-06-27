using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_Data.Migrations
{
    /// <inheritdoc />
    public partial class addeventidmeeting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "event_id",
                table: "meetings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_name",
                table: "users",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_name",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_username",
                table: "users");

            migrationBuilder.DropColumn(
                name: "event_id",
                table: "meetings");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
