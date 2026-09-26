using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DemoInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "inventory_key",
                schema: "automarket",
                table: "cars",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_featured",
                schema: "automarket",
                table: "cars",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_cars_inventory_key",
                schema: "automarket",
                table: "cars",
                column: "inventory_key",
                unique: true,
                filter: "inventory_key IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_cars_inventory_key",
                schema: "automarket",
                table: "cars");

            migrationBuilder.DropColumn(
                name: "inventory_key",
                schema: "automarket",
                table: "cars");

            migrationBuilder.DropColumn(
                name: "is_featured",
                schema: "automarket",
                table: "cars");
        }
    }
}
