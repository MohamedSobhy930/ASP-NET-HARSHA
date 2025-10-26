using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entities
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions options) : base(options) 
        { 

        }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            // seed from json 
            // 1- read from json
            if (!Database.IsInMemory())
            {
                string countriesjson = File.ReadAllText("Countries.json");
                string personsjson = File.ReadAllText("Persons.json");

                //2- deserialize from json to list 
                List<Country> countries = JsonSerializer.Deserialize<List<Country>>(countriesjson);
                List<Person> persons = JsonSerializer.Deserialize<List<Person>>(personsjson);

                //3- seed to database
                if (countries != null && countries.Any())
                {
                    modelBuilder.Entity<Country>().HasData(countries);
                }
                if (persons != null && persons.Any())
                {
                    modelBuilder.Entity<Person>().HasData(persons);
                }
            }
        }
        public int sp_InsertPerson(Person person)
        {
            var pPersonId = new SqlParameter("@PersonId",person.PersonId);
            var pName = new SqlParameter("@Name", person.Name ?? (object)DBNull.Value);
            var pEmail = new SqlParameter("@Email", person.Email ?? (object)DBNull.Value);
            var pDateOfBirth = new SqlParameter("@DateOfBirth", person.DateOfBirth ?? (object)DBNull.Value);
            var pGender = new SqlParameter("@Gender", person.Gender ?? (object)DBNull.Value);
            var pCountryId = new SqlParameter("@CountryId", person.CountryId ?? (object)DBNull.Value);
            var pAddress = new SqlParameter("@Address", person.Address ?? (object)DBNull.Value);
            var pReceiveNewsletter = new SqlParameter("@ReceiveNewsletter", person.ReceiveNewsletter ?? (object)DBNull.Value);

            return Database.ExecuteSqlRaw(
            "EXEC sp_InsertPerson @Name, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsletter, @PersonId",
            pName, pEmail, pDateOfBirth, pGender, pCountryId, pAddress, pReceiveNewsletter, pPersonId
            );
        }
    }
}
