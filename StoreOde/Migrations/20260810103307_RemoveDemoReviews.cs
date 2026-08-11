using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StoreOde.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDemoReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
