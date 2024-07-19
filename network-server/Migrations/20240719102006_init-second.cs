using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace network_server.Migrations
{
    /// <inheritdoc />
    public partial class initsecond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("21daedac-965a-439f-b2c5-49aded0d5564"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("233b6c61-677c-4c2d-9db2-c5ab5bc6580f"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("5b9855e6-4cf9-4db6-8b2b-900a718329d7"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { new Guid("21daedac-965a-439f-b2c5-49aded0d5564"), "User" },
                    { new Guid("233b6c61-677c-4c2d-9db2-c5ab5bc6580f"), "Employee" },
                    { new Guid("5b9855e6-4cf9-4db6-8b2b-900a718329d7"), "Admin" }
                });
        }
    }
}
