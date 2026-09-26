using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LicensedDemoPhotoMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "attribution",
                schema: "automarket",
                table: "car_images",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "public_url",
                schema: "automarket",
                table: "car_images",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "source_url",
                schema: "automarket",
                table: "car_images",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "attribution",
                schema: "automarket",
                table: "car_images");

            migrationBuilder.DropColumn(
                name: "public_url",
                schema: "automarket",
                table: "car_images");

            migrationBuilder.DropColumn(
                name: "source_url",
                schema: "automarket",
                table: "car_images");
        }
    }
}
