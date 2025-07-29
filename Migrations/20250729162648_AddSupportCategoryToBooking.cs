using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportBookingAPP.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportCategoryToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupportCategoryId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SupportCategoryId",
                table: "Bookings",
                column: "SupportCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_SupportCategories_SupportCategoryId",
                table: "Bookings",
                column: "SupportCategoryId",
                principalTable: "SupportCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_SupportCategories_SupportCategoryId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_SupportCategoryId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "SupportCategoryId",
                table: "Bookings");
        }
    }
}
