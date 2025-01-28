using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Migrations
{
    /// <inheritdoc />
    public partial class AddedPropertiesToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Requests");
        }
    }
}
