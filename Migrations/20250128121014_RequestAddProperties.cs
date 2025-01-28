using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Migrations
{
    /// <inheritdoc />
    public partial class RequestAddProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Requests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Requests_CompanyId",
                table: "Requests",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Companies_CompanyId",
                table: "Requests",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Companies_CompanyId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_CompanyId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Requests");
        }
    }
}
