using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Migrations
{
    /// <inheritdoc />
    public partial class RequestProductTableNewPropertiesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductBrand",
                table: "RequestProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductModel",
                table: "RequestProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductBrand",
                table: "RequestProducts");

            migrationBuilder.DropColumn(
                name: "ProductModel",
                table: "RequestProducts");
        }
    }
}
