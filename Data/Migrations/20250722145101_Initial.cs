using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SupportBookingAPP.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Engineers_EngineerId",
                table: "Bookings");

            migrationBuilder.DeleteData(
                table: "Engineers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Engineers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<string>(
                name: "IssueDescription",
                table: "Bookings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
                name: "FK_Bookings_AspNetUsers_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Engineers_EngineerId",
                table: "Bookings",
                column: "EngineerId",
                principalTable: "Engineers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Engineers_EngineerId1",
                table: "Bookings",
                column: "EngineerId1",
                principalTable: "Engineers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_ApplicationUserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Engineers_EngineerId",
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

            migrationBuilder.AlterColumn<string>(
                name: "IssueDescription",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.InsertData(
                table: "Engineers",
                columns: new[] { "Id", "Email", "Name", "Specialty", "WorkdayEnd", "WorkdayStart" },
                values: new object[,]
                {
                    { 1, "", "Alice Ivanova", "Networking", new DateTime(2025, 7, 22, 17, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 7, 22, 9, 0, 0, 0, DateTimeKind.Local) },
                    { 2, "", "Boris Petrov", "Hardware", new DateTime(2025, 7, 22, 18, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 7, 22, 10, 0, 0, 0, DateTimeKind.Local) }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Engineers_EngineerId",
                table: "Bookings",
                column: "EngineerId",
                principalTable: "Engineers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
