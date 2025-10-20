using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class alter_sp_GetAllPersons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER PROCEDURE sp_GetAllPersons
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.PersonId AS Id, 
        p.Name, 
        p.Email, 
        p.DateOfBirth, 
        p.Gender, 
        p.CountryId,
        c.Name AS Country,
        p.Address, 
        p.ReceiveNewsletter,
        
        -- This is the fixed line
        CAST(
            CASE 
                WHEN p.DateOfBirth IS NOT NULL 
                THEN FLOOR(DATEDIFF(day, p.DateOfBirth, GETDATE()) / 365.25)
                ELSE NULL 
            END 
        AS FLOAT) AS Age
        
    FROM 
        Persons p
    LEFT JOIN 
        Countries c ON p.CountryId = c.Id;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS sp_GetAllPersons");
        }
    }
}
