using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InsertPerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"CREATE PROCEDURE sp_InsertPerson
                    @Name VARCHAR(40),
                    @Email VARCHAR(40),
                    @DateOfBirth DATETIME2,
                    @Gender VARCHAR(10),
                    @CountryId UNIQUEIDENTIFIER,
                    @Address VARCHAR(100),
                    @ReceiveNewsletter BIT,
                    @PersonId UNIQUEIDENTIFIER
                AS BEGIN

                    INSERT INTO Persons (
                        PersonId, Name, Email, DateOfBirth, Gender, CountryId, Address, ReceiveNewsletter
                    )
                    VALUES (
                        @PersonId, @Name, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsletter
                    );
                END
            ";
            migrationBuilder.Sql(sp_InsertPerson);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_InsertPerson");
        }
    }
}
