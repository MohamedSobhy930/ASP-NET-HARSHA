using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class GetPersons_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
            CREATE PROCEDURE sp_GetAllPersons
                AS BEGIN

                SELECT 
                p.PersonId, p.Name, p.Email, p.DateOfBirth, p.Gender, p.CountryId, p.Address, p.ReceiveNewsletter, c.Name AS CountryName 
                FROM Persons p LEFT JOIN Countries c
                ON p.CountryId = c.Id;

                END
            ";
            migrationBuilder.Sql(sp_GetAllPersons);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
            DROP PROCEDURE IF EXISTS sp_GetAllPersons";
        }
    }
}
