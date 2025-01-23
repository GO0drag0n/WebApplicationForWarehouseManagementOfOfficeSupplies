using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestProductRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Categories_CategoryID",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_CategoryID",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Requests");

            migrationBuilder.CreateTable(
                name: "RequestProducts",
                columns: table => new
                {
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestProducts", x => new { x.RequestID, x.ProductID });
                    table.ForeignKey(
                        name: "FK_RequestProducts_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestProducts_Requests_RequestID",
                        column: x => x.RequestID,
                        principalTable: "Requests",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestProducts_ProductID",
                table: "RequestProducts",
                column: "ProductID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestProducts");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_CategoryID",
                table: "Requests",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Categories_CategoryID",
                table: "Requests",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
