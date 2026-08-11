using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreOde.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceCategoryPhotoWithIconClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(
            MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Category");

            migrationBuilder.AddColumn<string>(
                name: "IconClass",
                table: "Category",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconClass",
                table: "Category");

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "Category",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}