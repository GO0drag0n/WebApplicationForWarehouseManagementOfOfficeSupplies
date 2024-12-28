using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRequestAndRequestHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserID",
                table: "RequestHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RequestHistories_UpdatedByUserID",
                table: "RequestHistories",
                column: "UpdatedByUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHistories_Users_UpdatedByUserID",
                table: "RequestHistories",
                column: "UpdatedByUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestHistories_Users_UpdatedByUserID",
                table: "RequestHistories");

            migrationBuilder.DropIndex(
                name: "IX_RequestHistories_UpdatedByUserID",
                table: "RequestHistories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserID",
                table: "RequestHistories");
        }
    }
}
