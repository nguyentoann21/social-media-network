using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace network_server.Migrations
{
    /// <inheritdoc />
    public partial class initchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("bb5b1b23-791d-4a74-86b7-a1f3568c58d2"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("db5f1018-1962-469b-95e9-577b1f219a29"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("fc3a8566-98fb-4057-bdf6-ee700595f162"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { new Guid("56da879e-005a-471f-a7d8-1a7b8801000d"), "Employee" },
                    { new Guid("cb61de30-fe97-4b6d-b136-8a66e160b36c"), "Manager" },
                    { new Guid("e226b5c5-5bfe-4b16-9c7e-e11a0b2f9fa4"), "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("56da879e-005a-471f-a7d8-1a7b8801000d"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("cb61de30-fe97-4b6d-b136-8a66e160b36c"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("e226b5c5-5bfe-4b16-9c7e-e11a0b2f9fa4"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { new Guid("bb5b1b23-791d-4a74-86b7-a1f3568c58d2"), "User" },
                    { new Guid("db5f1018-1962-469b-95e9-577b1f219a29"), "Admin" },
                    { new Guid("fc3a8566-98fb-4057-bdf6-ee700595f162"), "Employee" }
                });
        }
    }
}
