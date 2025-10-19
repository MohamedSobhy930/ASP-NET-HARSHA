using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceiveNewsletter = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.PersonId);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("1a6b0c2a-9b4f-4a09-8b8e-8a2b0c6d7e0f"), "USA" },
                    { new Guid("8b5f3a0c-4d7e-4b9e-8b4f-0a8c1e6d7b2a"), "Egypt" },
                    { new Guid("c4a0b6e8-2d7c-4a0b-8d6f-1a9c2e7d8b3a"), "United Kingdom" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "PersonId", "Address", "CountryId", "DateOfBirth", "Email", "Gender", "Name", "ReceiveNewsletter" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-a7b8-c9d0-e1f2a3b4c5d6"), "456 Nile Corniche, Cairo", new Guid("8b5f3a0c-4d7e-4b9e-8b4f-0a8c1e6d7b2a"), new DateTime(1992, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "fatima.alsayed@example.com", "Female", "Fatima Al-Sayed", false },
                    { new Guid("e8b1b2a9-1d9c-4c6b-9f1e-3a7d5c9f2b8a"), "10 Downing St, London", new Guid("c4a0b6e8-2d7c-4a0b-8d6f-1a9c2e7d8b3a"), new DateTime(1970, 11, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "james.bond@mi6.gov.uk", "Male", "James Bond", false },
                    { new Guid("f1a2b3c4-5d6e-7f8a-9b0c-1d2e3f4a5b6c"), "123 Main St, New York", new Guid("1a6b0c2a-9b4f-4a09-8b8e-8a2b0c6d7e0f"), new DateTime(1985, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.smith@example.com", "Male", "John Smith", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
