using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CitiesManager.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class seedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("138f18e2-ea3b-41ef-927d-d007ba3d90ed"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("405c449b-91cf-4118-8ceb-4886c772fe10"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("94132256-03d6-4179-b67c-1af39d89c877"));

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("048fe215-906d-4f9c-b96d-a42ea6a93545"), "Chicago" },
                    { new Guid("7a7a85e0-ff23-49e2-8eb6-734ccde628f4"), "Los Angeles" },
                    { new Guid("dc0306a6-b04e-4af6-b276-20831ccdbf77"), "New York" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("048fe215-906d-4f9c-b96d-a42ea6a93545"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("7a7a85e0-ff23-49e2-8eb6-734ccde628f4"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("dc0306a6-b04e-4af6-b276-20831ccdbf77"));

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("138f18e2-ea3b-41ef-927d-d007ba3d90ed"), "Los Angeles" },
                    { new Guid("405c449b-91cf-4118-8ceb-4886c772fe10"), "Chicago" },
                    { new Guid("94132256-03d6-4179-b67c-1af39d89c877"), "New York" }
                });
        }
    }
}
