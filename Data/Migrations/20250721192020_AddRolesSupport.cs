using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportBookingAPP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "Engineers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "WorkdayEnd", "WorkdayStart" },
                values: new object[] { new DateTime(2025, 7, 11, 17, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 7, 11, 9, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.UpdateData(
                table: "Engineers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "WorkdayEnd", "WorkdayStart" },
                values: new object[] { new DateTime(2025, 7, 11, 18, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 7, 11, 10, 0, 0, 0, DateTimeKind.Local) });
        }
    }
}
