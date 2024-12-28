using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_UserID",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHistories_Requests_RequestID",
                table: "RequestHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHistories_Users_UpdatedByUserID",
                table: "RequestHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_UserID",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RequestHistories");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "RequestHistories");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserID",
                table: "RequestHistories",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "RequestID",
                table: "RequestHistories",
                newName: "RequestId");

            migrationBuilder.RenameColumn(
                name: "HistoryID",
                table: "RequestHistories",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHistories_UpdatedByUserID",
                table: "RequestHistories",
                newName: "IX_RequestHistories_UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHistories_RequestID",
                table: "RequestHistories",
                newName: "IX_RequestHistories_RequestId");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Requests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUserId",
                table: "RequestHistories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "AuditLogs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_AspNetUsers_UserID",
                table: "AuditLogs",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHistories_AspNetUsers_UpdatedByUserId",
                table: "RequestHistories",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHistories_Requests_RequestId",
                table: "RequestHistories",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "RequestID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_UserID",
                table: "Requests",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_AspNetUsers_UserID",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHistories_AspNetUsers_UpdatedByUserId",
                table: "RequestHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHistories_Requests_RequestId",
                table: "RequestHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_UserID",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "RequestHistories",
                newName: "UpdatedByUserID");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "RequestHistories",
                newName: "RequestID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "RequestHistories",
                newName: "HistoryID");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHistories_UpdatedByUserId",
                table: "RequestHistories",
                newName: "IX_RequestHistories_UpdatedByUserID");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHistories_RequestId",
                table: "RequestHistories",
                newName: "IX_RequestHistories_RequestID");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Requests",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedByUserID",
                table: "RequestHistories",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "RequestHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "RequestHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "AuditLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserID, x.RoleID });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleID",
                table: "UserRoles",
                column: "RoleID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_UserID",
                table: "AuditLogs",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHistories_Requests_RequestID",
                table: "RequestHistories",
                column: "RequestID",
                principalTable: "Requests",
                principalColumn: "RequestID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHistories_Users_UpdatedByUserID",
                table: "RequestHistories",
                column: "UpdatedByUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_UserID",
                table: "Requests",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
