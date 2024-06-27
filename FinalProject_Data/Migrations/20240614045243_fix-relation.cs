using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject_Data.Migrations
{
    /// <inheritdoc />
    public partial class fixrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meetings_users_owner_id",
                table: "meetings");

            migrationBuilder.AddForeignKey(
                name: "FK_meetings_users_owner_id",
                table: "meetings",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meetings_users_owner_id",
                table: "meetings");

            migrationBuilder.AddForeignKey(
                name: "FK_meetings_users_owner_id",
                table: "meetings",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
