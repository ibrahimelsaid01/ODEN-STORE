using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreOde.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "ClassFilter", "Description", "Name", "Photo" },
                values: new object[] { 16, null, "Products without a specified brand", "Other", null });

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 1,
                column: "Catid",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 2,
                column: "Catid",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 3,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 4,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 5,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 6,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 7,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 8,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 9,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 10,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 11,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 12,
                column: "Catid",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 13,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 14,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 15,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 16,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 17,
                column: "Catid",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 18,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 19,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 20,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 21,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 22,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 23,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 24,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 25,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 26,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 27,
                column: "Catid",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 28,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 29,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 30,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 31,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 32,
                column: "Catid",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 33,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 34,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 35,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 36,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 37,
                column: "Catid",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 38,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 39,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 40,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 41,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 42,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 43,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 44,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 45,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 46,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 47,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 48,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 49,
                column: "Catid",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 50,
                column: "Catid",
                value: 16);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 1,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 2,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 3,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 4,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 5,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 6,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 7,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 8,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 9,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 10,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 11,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 12,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 13,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 14,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 15,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 16,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 17,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 18,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 19,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 20,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 21,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 22,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 23,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 24,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 25,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 26,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 27,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 28,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 29,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 30,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 31,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 32,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 33,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 34,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 35,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 36,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 37,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 38,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 39,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 40,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 41,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 42,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 43,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 44,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 45,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 46,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 47,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 48,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 49,
                column: "Catid",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "id",
                keyValue: 50,
                column: "Catid",
                value: null);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 16);
        }
    }
}