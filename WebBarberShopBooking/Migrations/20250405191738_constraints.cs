using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBarberShopBooking.Migrations
{
    /// <inheritdoc />
    public partial class constraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_check_date",
                table: "Promotions",
                sql: "[EndDate] > [StartDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_check_promo",
                table: "Promotions",
                sql: "[DiscountPercentage] IS NULL OR ([DiscountPercentage] > 0 AND [DiscountPercentage] <= 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_check_date",
                table: "Promotions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_check_promo",
                table: "Promotions");
        }
    }
}
