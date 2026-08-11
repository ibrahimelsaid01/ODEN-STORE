using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreOde.Migrations
{
    public partial class SeedReviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "ClassFilter", "Description", "Name", "Photo" },
                values: new object[,]
                {
                    { 1, null, "Top sports brand", "Nike", null },
                    { 2, null, "Premium sportswear", "Adidas", null },
                    { 3, null, "Comfort & style", "Puma", null },
                    { 4, null, "Fitness focused", "Reebok", null },
                    { 5, null, "Performance gear", "Under Armour", null },
                    { 6, null, "Running excellence", "New Balance", null },
                    { 7, null, "Classic sportswear", "Champion", null },
                    { 8, null, "Retro sports fashion", "Fila", null },
                    { 9, null, "Professional running gear", "Asics", null },
                    { 10, null, "Italian sports style", "Lotto", null },
                    { 11, null, "Football heritage brand", "Umbro", null },
                    { 12, null, "Sport & lifestyle wear", "Diadora", null },
                    { 13, null, "Street sports fashion", "Kappa", null },
                    { 14, null, "Outdoor & adventure gear", "Columbia", null },
                    { 15, null, "Extreme weather performance", "The North Face", null }
                });

            migrationBuilder.InsertData(
                table: "Review",
                columns: new[] { "Id", "Description", "Email", "Name", "Subject" },
                values: new object[,]
                {
                    { 1, "Amazing quality and fast delivery!", "ahmed@gmail.com", "Ahmed Hassan", null },
                    { 2, "Original products and great prices.", "sara@gmail.com", "Sara Mohamed", null },
                    { 3, "Very comfortable and high quality.", "omar@gmail.com", "Omar Ali", null },
                    { 4, "I loved the hoodie, perfect fit!", "mona@gmail.com", "Mona Adel", null },
                    { 5, "Fast delivery and excellent service.", "khaled@gmail.com", "Khaled Mostafa", null },
                    { 6, "Products are exactly as shown in pictures.", "nour@gmail.com", "Nour Ahmed", null },
                    { 7, "Very comfortable shoes, highly recommended.", "youssef@gmail.com", "Youssef Ali", null },
                    { 8, "Great quality and affordable prices.", "salma@gmail.com", "Salma Tarek", null },
                    { 9, "Best sportswear store I’ve tried.", "mohamed@gmail.com", "Mohamed Gamal", null },
                    { 10, "Amazing experience, will buy again.", "heba@gmail.com", "Heba Mostafa", null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Review",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Review",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Review",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Review",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Review",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Review",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Review",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Review",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Review",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Review",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
