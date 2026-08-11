using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreOde.Migrations
{
    public partial class Seed50Products : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "id", "Catid", "Description", "EntryDate", "Name", "Photo", "Price", "priceafterdiscount", "quantity", "ReviewUrl", "SupplierName", "type" },
                values: new object[,]
                {
                    { 1, null, null, null, "Nike Hoodie Black", null, 1200m, null, null, null, null, "hoodie" },
                    { 2, null, null, null, "Adidas Running Shoes", null, 2000m, null, null, null, null, "shoes" },
                    { 3, null, null, null, "Sport Shorts Blue", null, 500m, null, null, null, null, "shorts" },
                    { 4, null, null, null, "Winter Jacket Premium", null, 1500m, null, null, null, null, "jacket" },
                    { 5, null, null, null, "Slim Fit Pants", null, 900m, null, null, null, null, "pants" },
                    { 6, null, null, null, "Classic White Hoodie", null, 1100m, null, null, null, null, "hoodie" },
                    { 7, null, null, null, "Air Max Sneakers", null, 2500m, null, null, null, null, "shoes" },
                    { 8, null, null, null, "Gym Shorts Grey", null, 450m, null, null, null, null, "shorts" },
                    { 9, null, null, null, "Leather Jacket Black", null, 3000m, null, null, null, null, "jacket" },
                    { 10, null, null, null, "Jogger Pants", null, 800m, null, null, null, null, "pants" },
                    { 11, null, null, null, "Oversized Hoodie Red", null, 1300m, null, null, null, null, "hoodie" },
                    { 12, null, null, null, "Adidas Ultra Boost", null, 2800m, null, null, null, null, "shoes" },
                    { 13, null, null, null, "Training Shorts", null, 550m, null, null, null, null, "shorts" },
                    { 14, null, null, null, "Denim Jacket", null, 1700m, null, null, null, null, "jacket" },
                    { 15, null, null, null, "Cargo Pants", null, 1000m, null, null, null, null, "pants" },
                    { 16, null, null, null, "Street Hoodie Grey", null, 1250m, null, null, null, null, "hoodie" },
                    { 17, null, null, null, "Nike Air Force", null, 2600m, null, null, null, null, "shoes" },
                    { 18, null, null, null, "Summer Shorts", null, 400m, null, null, null, null, "shorts" },
                    { 19, null, null, null, "Windbreaker Jacket", null, 1400m, null, null, null, null, "jacket" },
                    { 20, null, null, null, "Sport Jogger", null, 850m, null, null, null, null, "pants" },
                    { 21, null, null, null, "Hoodie Zip Black", null, 1350m, null, null, null, null, "hoodie" },
                    { 22, null, null, null, "Running Shoes Pro", null, 2200m, null, null, null, null, "shoes" },
                    { 23, null, null, null, "Fitness Shorts", null, 480m, null, null, null, null, "shorts" },
                    { 24, null, null, null, "Puffer Jacket", null, 3200m, null, null, null, null, "jacket" },
                    { 25, null, null, null, "Casual Pants Beige", null, 950m, null, null, null, null, "pants" },
                    { 26, null, null, null, "Tech Hoodie Blue", null, 1450m, null, null, null, null, "hoodie" },
                    { 27, null, null, null, "Nike Zoom Shoes", null, 2400m, null, null, null, null, "shoes" },
                    { 28, null, null, null, "Sport Shorts Black", null, 520m, null, null, null, null, "shorts" },
                    { 29, null, null, null, "Bomber Jacket", null, 1800m, null, null, null, null, "jacket" },
                    { 30, null, null, null, "Slim Jogger Grey", null, 870m, null, null, null, null, "pants" },
                    { 31, null, null, null, "Hoodie Oversize White", null, 1500m, null, null, null, null, "hoodie" },
                    { 32, null, null, null, "Adidas Originals Shoes", null, 2700m, null, null, null, null, "shoes" },
                    { 33, null, null, null, "Running Shorts Green", null, 460m, null, null, null, null, "shorts" },
                    { 34, null, null, null, "Winter Coat Long", null, 3500m, null, null, null, null, "jacket" },
                    { 35, null, null, null, "Track Pants Black", null, 920m, null, null, null, null, "pants" },
                    { 36, null, null, null, "Streetwear Hoodie", null, 1600m, null, null, null, null, "hoodie" },
                    { 37, null, null, null, "Nike Casual Shoes", null, 2300m, null, null, null, null, "shoes" },
                    { 38, null, null, null, "Gym Shorts Red", null, 500m, null, null, null, null, "shorts" },
                    { 39, null, null, null, "Rain Jacket", null, 1900m, null, null, null, null, "jacket" },
                    { 40, null, null, null, "Cotton Pants", null, 780m, null, null, null, null, "pants" },
                    { 41, null, null, null, "Basic Hoodie Grey", null, 1000m, null, null, null, null, "hoodie" },
                    { 42, null, null, null, "Sport Shoes Lite", null, 2100m, null, null, null, null, "shoes" }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "id", "Catid", "Description", "EntryDate", "Name", "Photo", "Price", "priceafterdiscount", "quantity", "ReviewUrl", "SupplierName", "type" },
                values: new object[,]
                {
                    { 43, null, null, null, "Training Shorts Pro", null, 530m, null, null, null, null, "shorts" },
                    { 44, null, null, null, "Leather Coat", null, 4000m, null, null, null, null, "jacket" },
                    { 45, null, null, null, "Relax Pants", null, 820m, null, null, null, null, "pants" },
                    { 46, null, null, null, "Urban Hoodie", null, 1400m, null, null, null, null, "hoodie" },
                    { 47, null, null, null, "Running Sneakers Elite", null, 3000m, null, null, null, null, "shoes" },
                    { 48, null, null, null, "Sport Shorts Pro", null, 600m, null, null, null, null, "shorts" },
                    { 49, null, null, null, "Parka Jacket", null, 3800m, null, null, null, null, "jacket" },
                    { 50, null, null, null, "Cargo Jogger Pants", null, 1100m, null, null, null, null, "pants" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "id",
                keyValue: 50);
        }
    }
}
