using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SupportBookingAPP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEngineerEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EquipmentId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Engineers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkdayStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkdayEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Engineers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EngineerId = table.Column<int>(type: "int", nullable: false),
                    SlotStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SlotEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssueDescription = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Engineers_EngineerId",
                        column: x => x.EngineerId,
                        principalTable: "Engineers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Engineers",
                columns: new[] { "Id", "Email", "Name", "Specialty", "WorkdayEnd", "WorkdayStart" },
                values: new object[,]
                {
                    { 1, "", "Alice Ivanova", "Networking", new DateTime(2025, 7, 11, 17, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 7, 11, 9, 0, 0, 0, DateTimeKind.Local) },
                    { 2, "", "Boris Petrov", "Hardware", new DateTime(2025, 7, 11, 18, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 7, 11, 10, 0, 0, 0, DateTimeKind.Local) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_EngineerId",
                table: "Bookings",
                column: "EngineerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Engineers");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "AspNetUsers");
        }
    }
}
