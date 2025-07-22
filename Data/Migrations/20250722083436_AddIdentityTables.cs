using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportBookingAPP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Engineers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "WorkdayEnd", "WorkdayStart" },
                values: new object[] { new DateTime(2025, 7, 22, 17, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 7, 22, 9, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Engineers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "WorkdayEnd", "WorkdayStart" },
                values: new object[] { new DateTime(2025, 7, 22, 18, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 7, 22, 10, 0, 0, 0, DateTimeKind.Local) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Engineers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "WorkdayEnd", "WorkdayStart" },
                values: new object[] { new DateTime(2025, 7, 21, 17, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 7, 21, 9, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Engineers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "WorkdayEnd", "WorkdayStart" },
                values: new object[] { new DateTime(2025, 7, 21, 18, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 7, 21, 10, 0, 0, 0, DateTimeKind.Local) });
        }
    }
}
