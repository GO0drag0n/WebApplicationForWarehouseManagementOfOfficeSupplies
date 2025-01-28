using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Migrations
{
    /// <inheritdoc />
    public partial class AddRowAndSectionToRequestProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductRow",
                table: "RequestProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductSection",
                table: "RequestProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductRow",
                table: "RequestProducts");

            migrationBuilder.DropColumn(
                name: "ProductSection",
                table: "RequestProducts");
        }
    }
}
