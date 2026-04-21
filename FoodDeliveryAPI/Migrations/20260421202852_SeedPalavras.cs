using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodDeliveryAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedPalavras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PalavrasProibidas",
                columns: new[] { "Id", "Palavra" },
                values: new object[,]
                {
                    { 1, "xxx" },
                    { 2, "idiota" },
                    { 3, "burro" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PalavrasProibidas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PalavrasProibidas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PalavrasProibidas",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
