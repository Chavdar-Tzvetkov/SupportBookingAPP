using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportBookingAPP.Migrations
{
    /// <inheritdoc />
    public partial class FixEngineerIdOnApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_ApplicationUserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Engineers_EngineerId1",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ApplicationUserId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_EngineerId1",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "EngineerId1",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "EngineerId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EngineerId",
                table: "AspNetUsers",
                column: "EngineerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Engineers_EngineerId",
                table: "AspNetUsers",
                column: "EngineerId",
                principalTable: "Engineers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Engineers_EngineerId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EngineerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EngineerId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EngineerId1",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ApplicationUserId",
                table: "Bookings",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_EngineerId1",
                table: "Bookings",
                column: "EngineerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_ApplicationUserId",
                table: "Bookings",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Engineers_EngineerId1",
                table: "Bookings",
                column: "EngineerId1",
                principalTable: "Engineers",
                principalColumn: "Id");
        }
    }
}
