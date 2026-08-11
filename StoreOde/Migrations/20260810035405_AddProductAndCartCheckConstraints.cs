using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreOde.Migrations
{
    /// <inheritdoc />
    public partial class AddProductAndCartCheckConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Product_Price_Positive",
                table: "Product",
                sql: "[Price] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Product_PriceAfterDiscount_Range",
                table: "Product",
                sql: "[priceafterdiscount] IS NULL OR ([priceafterdiscount] >= 0 AND [priceafterdiscount] <= [Price])");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Product_Quantity_NonNegative",
                table: "Product",
                sql: "[quantity] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Cart_Qty_Positive",
                table: "Cart",
                sql: "[Qty] > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Product_Price_Positive",
                table: "Product");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Product_PriceAfterDiscount_Range",
                table: "Product");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Product_Quantity_NonNegative",
                table: "Product");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Cart_Qty_Positive",
                table: "Cart");
        }
    }
}
