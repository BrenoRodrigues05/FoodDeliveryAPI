using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDeliveryAPI.Migrations
{
    /// <inheritdoc />
    public partial class criaCampoEmailEntregador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Entregadores",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Entregadores");
        }
    }
}
