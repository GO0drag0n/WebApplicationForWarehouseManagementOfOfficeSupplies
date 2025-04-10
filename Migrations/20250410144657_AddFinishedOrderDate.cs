using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Migrations
{
    /// <inheritdoc />
    public partial class AddFinishedOrderDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedOrderDate",
                table: "Requests",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishedOrderDate",
                table: "Requests");
        }
    }
}
